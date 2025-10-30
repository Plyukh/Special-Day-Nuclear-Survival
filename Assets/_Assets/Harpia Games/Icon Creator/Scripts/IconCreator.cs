using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if ENABLE_INPUT_SYSTEM
//If there's an error here, remember to add input system to assembly definition
//Go to to Documentation -> Using the new input system 
//https://docs.google.com/document/d/1O7FnBUAFJEZwadJSbIgfp5peQOi2QJfD_77_FMJ_i8g/
using UnityEngine.InputSystem;
#endif

namespace IconCreator
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent()]
    public class IconCreator : MonoBehaviour
    {

        /// <summary>
        /// An Enum to help saving the icons
        /// </summary>
        public enum SaveLocation
        {
            persistentDataPath,
            dataPath,
            projectFolder,
            custom
        }

        /// <summary>
        /// The mode enum to help personalizing the icons
        /// </summary>
        public enum Mode
        {
            Automatic,
            Manual
        }

        /// <summary>
        /// Is icons being created?
        /// </summary>
        protected bool isCreatingIcons;

        /// <summary>
        /// If can create an icon
        /// </summary>
        protected bool canCreateIcon;

        /// <summary>
        /// Should the icon have the default name?
        /// </summary>
        public bool useDafaultName;

        /// <summary>
        /// Should the icon have the resolution inside the name?
        /// </summary>
        public bool includeResolutionInFileName;

        /// <summary>
        /// The icon default name (only if useDafaultName is true)
        /// </summary>
        public string iconFileName;

        /// <summary>
        /// The location to save the icon
        /// </summary>
        public SaveLocation pathLocation;

        /// <summary>
        ///  The mode variable to help personalizing the icons
        /// </summary>
        public Mode mode;

        /// <summary>
        /// The icons parent folder name
        /// </summary>
        public string folderName = "Screenshots";

        /// <summary>
        /// Attention: this option will set that camera's Clear Flags field to Solid Color
        /// </summary>
        public bool useTransparency = true;

        /// <summary>
        /// Look at the object mesh center
        /// </summary>
        public bool lookAtObjectCenter;

        /// <summary>
        /// Try to find the best FOV value to the object fit the whole image file"
        /// </summary>
        public bool dynamicFov;

        /// <summary>
        /// Adds to the fov calculation.
        /// </summary>
        public float fovOffset = 0;

        /// <summary>
        /// The final path to save the icon
        /// </summary>
        protected string finalPath;

        /// <summary>
        /// The mouse position, used in manual mode.
        /// </summary>
        private Vector3 mousePostion;

        /// <summary>
        /// Do you want to move you camera?
        /// </summary>
        public bool cameraMoviment = false;

#if ENABLE_LEGACY_INPUT_MANAGER
        /// <summary>
        /// The key to next icon, used in manual mode.
        /// </summary>
        public KeyCode nextIconKeyLegacy = KeyCode.Space;

        /// <summary>
        /// The key to  force create an icon, used in manual mode.
        /// </summary>
        public KeyCode forceIconLegacy = KeyCode.UpArrow;


        /// <summary>
        /// Camera UP
        /// </summary>
        public KeyCode cameraGoUpLegacy = KeyCode.E;


        /// <summary>
        /// Camera Down
        /// </summary>
        public KeyCode cameraGoDownLegacy = KeyCode.Q;


        

#endif

#if ENABLE_INPUT_SYSTEM
        /// <summary>
        /// The key to next icon, used in manual mode.
        /// </summary>
        public Key nextIconKey = Key.Space;

        /// <summary>
        /// The key to  force create an icon, used in manual mode.
        /// </summary>
        public Key forceIcon = Key.UpArrow;

        /// <summary>
        /// Camera UP
        /// </summary>
        public Key cameraGoUp = Key.E;


        /// <summary>
        /// Camera Down
        /// </summary>
        public Key cameraGoDown = Key.Q;
#endif

        /// <summary>
        /// Can the user move the object? used in manual mode.
        /// </summary>
        protected bool CanMove;

        /// <summary>
        /// Can update camera props in edit mode?
        /// </summary>
        public bool preview = true;

        // Cameras to save the icon with transparency

        protected Camera whiteCam;
        protected Camera blackCam;
        public Camera mainCam;

        // Textures to save the icon with transparency

        protected Texture2D texBlack;
        protected Texture2D texWhite;
        protected Texture2D finalTexture;

        /// <summary>
        /// The original camera clear flags
        /// </summary>
        private CameraClearFlags originalClearFlags;

        /// <summary>
        /// The object to make an icon
        /// </summary>
        protected Transform currentObject;

        /// <summary>
        /// If can rotate the object, if the cursor is over selected UI you can't rotate the object
        /// </summary>
        bool canRotate = true;

        /// <summary>
        /// The current object name, used in icon file's name
        /// </summary>
        private string _currentObjectName;

        protected string currentObjectName
        {
            get { return _currentObjectName; }
            set {
                value = value.Replace("(Clone)", "");
                _currentObjectName = value; }
        }





        /// <summary>
        /// Verify if the the mouse button was pressed last frame
        /// </summary>
        bool wasPressed;


        private void Awake()
        {
            mainCam = gameObject.GetComponent<Camera>();

            originalClearFlags = mainCam.clearFlags;

#if ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(0, 0, "", false, nextIconKey.ToString(), forceIcon.ToString(), nextIconKeyLegacy.ToString());
#elif ENABLE_INPUT_SYSTEM
            if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(0, 0, "", false, nextIconKey.ToString(), forceIcon.ToString(),"");
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(0, 0, "", false, nextIconKeyLegacy.ToString(), forceIconLegacy.ToString(),"");
#endif
        }

        /// <summary>
        /// Initialize the cameras and the fodlers
        /// </summary>
        protected void Initialize()
        {
            mainCam.clearFlags = originalClearFlags;
            isCreatingIcons = true;
            canCreateIcon = true;

            IconCreatorCanvas canvas = GameObject.FindObjectOfType<IconCreatorCanvas>();

            if (!System.Object.ReferenceEquals(canvas, null))
            {
                canvas.BuildIconsBtn.gameObject.SetActive(false);
            }

         

            foreach (Camera item in GameObject.FindObjectsOfType<Camera>())
            {
                if (item == mainCam) continue;

                item.gameObject.SetActive(false);
            }

            if (useTransparency)
            {
                CreateBlackAndWhiteCameras();
            }

            CreateNewFolderForIcons();
            CacheAndInitialiseFields();

            
        }

        public void DestroyAllIconCreatorObjects()
        {
            foreach (var item in GameObject.FindObjectsOfType<GameObject>())
            {
                if(item.name.Contains("(Icon creator item)"))
                {
                    if (Application.isPlaying == false) {
                        StartCoroutine(Destroy(item.gameObject));
                    } else
                    {
                        Destroy(item.gameObject);
                    }
                }
            }
        }

        protected IEnumerator Destroy(GameObject go)
        {
            yield return new WaitForEndOfFrame();
            DestroyImmediate(go);
            DestroyInstances();
        }

        protected IEnumerator DelayedUpdate()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Update();
        }

        protected virtual void DestroyInstances()
        {

        }

        /// <summary>
        /// Delete white and black camera
        /// </summary>
        protected void DeleteCameras()
        {
            if (whiteCam != null) Destroy(whiteCam.gameObject);
            if (blackCam != null) Destroy(blackCam.gameObject);
            isCreatingIcons = false;
        }

        public virtual void BuildIcons()
        {
            Debug.LogError("Not implemented");
        }

        /// <summary>
        /// Capture one frame from the game view
        /// </summary>
        /// <param name="objectName">The name of the object to capture</param>
        /// <param name="i">A counter to evade same names</param>
        protected IEnumerator CreateIconRotine(string objectName, int i)
        {
            if (!canCreateIcon)
            {
                yield break;
            }

            canCreateIcon = false;

            if (whiteCam != null) whiteCam.enabled = true;
            if (blackCam != null) blackCam.enabled = true;

            yield return new WaitForEndOfFrame();

            if (useTransparency)
            {
                RenderCamToTexture(blackCam, texBlack);
                RenderCamToTexture(whiteCam, texWhite);
                CalculateOutputTexture();
            }
            else
            {
                RenderCamToTexture(mainCam, finalTexture);
            }

            SavePng(objectName, i);

            mainCam.enabled = true;

            canCreateIcon = true;
            Debug.Log("Icon created!");
        }

        protected virtual void Update()
        {
            if (currentObject != null)
            {
                if (lookAtObjectCenter) LookAtTargetCenter(currentObject.gameObject);
                if (dynamicFov) UpdateFOV(currentObject.gameObject);
            }

            if (mode == Mode.Automatic) return;

#if ENABLE_LEGACY_INPUT_MANAGER

            if (isCreatingIcons)
            {
                if (Input.GetKeyDown(forceIconLegacy))
                {
                    StartCoroutine(CreateIconRotine(currentObjectName, 0));
                }

                if (cameraMoviment)
                {
                    if (Input.GetKey(cameraGoUpLegacy))
                    {
                        transform.Translate(Vector3.up * Time.deltaTime * 2, Space.World);
                    }

                    if (Input.GetKey(cameraGoDownLegacy))
                    {
                        transform.Translate(Vector3.down * Time.deltaTime * 2, Space.World);
                    }
                }

            }



#elif ENABLE_INPUT_SYSTEM

            if (isCreatingIcons || Application.isPlaying)
            {
                if (Keyboard.current[forceIcon].wasPressedThisFrame)
                {
                    StartCoroutine(CreateIconRotine(currentObjectName, -1));
                }
                if (cameraMoviment)
                {
                    if (Keyboard.current[cameraGoUp].isPressed)
                    {
                        transform.Translate(Vector3.up * Time.deltaTime * 2, Space.World);
                    }

                    if (Keyboard.current[cameraGoDown].isPressed)
                    {
                        transform.Translate(Vector3.down * Time.deltaTime * 2, Space.World);
                    }
                }
            }

#endif

            if (!CanMove) return;
            if (Application.isFocused == false) return;

#if ENABLE_LEGACY_INPUT_MANAGER

            if (Input.GetMouseButtonDown(0))
            {
                mousePostion = Input.mousePosition;
            }

            if (Input.GetMouseButton(0) && canRotate)
            {
                Vector3 rot = mousePostion - Input.mousePosition;

                rot.z = 0;

                Vector3 relativeUp = transform.TransformDirection(Vector3.up);
                Vector3 relativeRight = transform.TransformDirection(Vector3.right);

                Vector3 objectRelativeUp = currentObject.InverseTransformDirection(relativeUp);
                Vector3 objectRelaviveRight = currentObject.InverseTransformDirection(relativeRight);

               Quaternion rotateBy = Quaternion.AngleAxis(rot.x / gameObject.transform.localScale.x * .5f, objectRelativeUp)
                 *  Quaternion.AngleAxis(-rot.y / gameObject.transform.localScale.x * .5f, objectRelaviveRight);

                currentObject.Rotate(rotateBy.eulerAngles);

                mousePostion = Input.mousePosition;

                if (dynamicFov) UpdateFOV(currentObject.gameObject);
                if (lookAtObjectCenter) LookAtTargetCenter(currentObject.gameObject);
            }


            UpdateFOV(Input.mouseScrollDelta.y * -2.5f);
#endif

#if ENABLE_INPUT_SYSTEM
            if (Mouse.current.leftButton.isPressed && !wasPressed )
            {
                mousePostion = Mouse.current.position.ReadValue();
            }

            if (Mouse.current.leftButton.isPressed && canRotate)
            {
                Vector2 p = Mouse.current.position.ReadValue();
                Vector3 rot = mousePostion - new Vector3( p.x, p.y);

                Vector3 relativeUp = transform.TransformDirection(Vector3.up);
                Vector3 relativeRight = transform.TransformDirection(Vector3.right);

                Vector3 objectRelativeUp = currentObject.InverseTransformDirection(relativeUp);
                Vector3 objectRelaviveRight = currentObject.InverseTransformDirection(relativeRight);

               Quaternion rotateBy = Quaternion.AngleAxis(rot.x / gameObject.transform.localScale.x * .5f, objectRelativeUp)
                 *  Quaternion.AngleAxis(-rot.y / gameObject.transform.localScale.x * .5f, objectRelaviveRight);

                currentObject.Rotate(rotateBy.eulerAngles);


                mousePostion = Mouse.current.position.ReadValue();

            }

            wasPressed = Mouse.current.leftButton.isPressed;
            UpdateFOV(Mouse.current.scroll.ReadValue().y * -1);
#endif

        }

        /// <summary>
        /// Set
        /// </summary>
        public void SetCanRotate(bool n)
        {
            canRotate = n;
        }

        /// <summary>
        /// Renders the camera view to a texture
        /// </summary>
        /// <param name="cam">The camera</param>
        /// <param name="tex">The target texture</param>
        private void RenderCamToTexture(Camera cam, Texture2D tex)
        {
            cam.enabled = true;
            cam.Render();
            WriteScreenImageToTexture(tex);
            cam.enabled = false;
        }

        /// <summary>
        /// Creates additional cameras (to use transparency)
        /// </summary>
        private void CreateBlackAndWhiteCameras()
        {
            mainCam.clearFlags = CameraClearFlags.SolidColor;

            GameObject aux = (GameObject)new GameObject();
            aux.name = "White Background Camera";
            whiteCam = aux.AddComponent<Camera>();
            whiteCam.CopyFrom(mainCam);
            whiteCam.backgroundColor = Color.white;
            aux.transform.SetParent(gameObject.transform, true);

            aux = (GameObject)new GameObject();
            aux.name = "Black Background Camera";
            blackCam = aux.AddComponent<Camera>();
            blackCam.CopyFrom(mainCam);
            blackCam.backgroundColor = Color.black;
            aux.transform.SetParent(gameObject.transform, true);
        }

        /// <summary>
        /// Creates the icons folder
        /// </summary>
        protected void CreateNewFolderForIcons()
        {
            finalPath = GetFinalFolder();

            if (System.IO.Directory.Exists(finalPath))
            {
                int count = 1;
                while (System.IO.Directory.Exists(finalPath + " " + count))
                {
                    count++;
                }

                finalPath = finalPath + " " + count;
            }

            System.IO.Directory.CreateDirectory(finalPath);
        }

        /// <summary>
        /// Generates  the folder to save the icons
        /// </summary>
        /// <returns>The folder to save the icons</returns>
        public string GetFinalFolder()
        {
            if (!string.IsNullOrWhiteSpace(GetBaseLocation()))
            {
                return Path.Combine(GetBaseLocation(), folderName);
            }

            return folderName;
        }

        /// <summary>
        /// Saves the the texture
        /// </summary>
        /// <param name="tex"></param>
        private void WriteScreenImageToTexture(Texture2D tex)
        {
            tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.width), 0, 0);
            tex.Apply();
        }

        /// <summary>
        /// Calculation for transparency
        /// </summary>
        private void CalculateOutputTexture()
        {
            Color color;
            for (int y = 0; y < finalTexture.height; ++y)
            {
                for (int x = 0; x < finalTexture.width; ++x)
                {
                    float a = texWhite.GetPixel(x, y).r - texBlack.GetPixel(x, y).r;
                    a = 1.0f - a;

                    if (a == 0) color = Color.clear;
                    else color = texBlack.GetPixel(x, y) / a;

                    color.a = a;
                    finalTexture.SetPixel(x, y, color);
                }
            }
        }

        /// <summary>
        /// Saves a PNG file
        /// </summary>
        /// <param name="name">The file name</param>
        /// <param name="i">Number to evade same names</param>
        private void SavePng(string name, int i)
        {
            string finalName = GetFileName(name, i);

            string path = finalPath + "/" + finalName;

            if (File.Exists(path))
            {
                SavePng(name, ++i);
                return;
            }

            var pngShot = finalTexture.EncodeToPNG();
            File.WriteAllBytes(path, pngShot);
        }

        /// <summary>
        /// Returns the file name
        /// </summary>
        /// <param name="name">Object name</param>
        /// <param name="i">umber to evade same names</param>
        /// <returns>The file name</returns>
        public string GetFileName(string name, int i)
        {
            string finalName;

            if (useDafaultName)
                finalName = name;
            else
            {
                finalName = iconFileName;
            }

            finalName += " " + i;

            if (includeResolutionInFileName) finalName += " " + mainCam.scaledPixelHeight + "x";
            return finalName + ".png";
        }

        /// <summary>
        /// Initialize and cache the textures, for performance propose
        /// </summary>
        private void CacheAndInitialiseFields()
        {
            texBlack = new Texture2D(mainCam.pixelWidth, mainCam.pixelHeight, TextureFormat.RGB24, false);
            texWhite = new Texture2D(mainCam.pixelWidth, mainCam.pixelHeight, TextureFormat.RGB24, false);
            finalTexture = new Texture2D(mainCam.pixelWidth, mainCam.pixelHeight, TextureFormat.ARGB32, false);
        }

        /// <summary>
        /// Updates the fov value to fit the object
        /// </summary>
        /// <param name="targetItem">the object</param>
        protected void UpdateFOV(GameObject targetItem)
        {
            float f = GetTargetFov(targetItem);

            if (useTransparency && isCreatingIcons)
            {
                whiteCam.fieldOfView = f;
                blackCam.fieldOfView = f;
            }

            mainCam.fieldOfView = f;
        }

        /// <summary>
        /// Updates the fov when using the mouse scroll. Used in manual mode.
        /// </summary>
        /// <param name="value"></param>
        protected void UpdateFOV(float value)
        {            

            if (value == 0) return;

            float sign = -1;
            if (value > 0) sign = 1;

            value = Mathf.Clamp( Mathf.Abs(value), 0, 5)  * sign;

            value = mainCam.fieldOfView * value / 100;

            dynamicFov = false;
            if (useTransparency)
            {
                whiteCam.fieldOfView += value;
                blackCam.fieldOfView += value;
            }

            mainCam.fieldOfView += value;
        }

        /// <summary>
        /// Look at at the center of the target item
        /// </summary>
        /// <param name="targetItem">The item to icon</param>
        protected void LookAtTargetCenter(GameObject targetItem)
        {
            Vector3 pos = GetMeshCenter(targetItem);
            mainCam.transform.LookAt(pos);
            if (whiteCam != null) whiteCam.transform.LookAt(pos);
            if (blackCam != null) blackCam.transform.LookAt(pos);
        }

        /// <summary>
        /// Calculates the desirable fov
        /// </summary>
        /// <param name="a"></param>
        private float GetTargetFov(GameObject a)
        {
            Vector3 boundsMin = Vector3.one * 30000;
            Vector3 boundsMax = Vector3.zero;

            List<Renderer> renderers = GetRenderers(a);

            for (int i = 0; i < renderers.Count; i++)
            {
                if (Vector3.Distance(Vector3.zero, renderers[i].bounds.min) < Vector3.Distance(Vector3.zero, boundsMin))
                {
                    boundsMin = renderers[i].bounds.min;
                }

                if (Vector3.Distance(Vector3.zero, renderers[i].bounds.max) > Vector3.Distance(Vector3.zero, boundsMax))
                {
                    boundsMax = renderers[i].bounds.max;
                }
            }

            Vector3 center = (boundsMin + boundsMax) / 2;

            float r = (boundsMax - boundsMin).magnitude / 2;
            float d = Vector3.Distance(center, transform.position);
            float c = Mathf.Sqrt(r * r + d * d);

            return Mathf.Asin(r / c) * Mathf.Rad2Deg * 2f + fovOffset;
        }

        /// <summary>
        /// Returns all renderes of an object.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>a list with all renderers</returns>
        private List<Renderer> GetRenderers(GameObject obj)
        {
            List<Renderer> renderers = new List<Renderer>();

            if (obj.GetComponents<Renderer>() != null) renderers.AddRange(obj.GetComponents<Renderer>());
            if (obj.GetComponentsInChildren<Renderer>() != null) renderers.AddRange(obj.GetComponentsInChildren<Renderer>());

            return renderers;
        }

        /// <summary>
        /// Get the center of the item mesh
        /// </summary>
        /// <param name="a">the prefab</param>
        /// <returns>the center of the item mesh </returns>
        private Vector3 GetMeshCenter(GameObject a)
        {
            Vector3 sum = Vector3.zero;
            List<Renderer> renderers = GetRenderers(a);

            if (renderers == null)
            {
                Debug.LogError("No mesh was founded in object " + a.name);
                return a.transform.position;
            }

            for (int i = 0; i < renderers.Count; i++)
            {
                sum += renderers[i].bounds.center;
            }

            sum /= renderers.Count;

            return sum;
        }

        /// <summary>
        /// Open the file explorer (Windows) or the Finder (MacOS)
        /// </summary>
        protected void RevealInFinder()
        {
#if UNITY_EDITOR
            EditorUtility.RevealInFinder(finalPath + "/");
#endif

        }

        /// <summary>
        /// Check if can start making the icons
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckConditions()
        {
            if (pathLocation == SaveLocation.custom)
            {
                if (!Directory.Exists(folderName))
                {
                    Debug.LogError($"Folder {folderName} does not exists");
                    return false;
                }
            }

            if (!useDafaultName)
            {
                if (string.IsNullOrWhiteSpace(iconFileName))
                {
                    Debug.LogError("Invalid icon file name");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Generate the icons's root folders location
        /// </summary>
        /// <returns>The icons's root folders location </returns>
        private string GetBaseLocation()
        {
            if (pathLocation == SaveLocation.dataPath) return Application.dataPath;
            if (pathLocation == SaveLocation.persistentDataPath) return Application.persistentDataPath;
            if (pathLocation == SaveLocation.projectFolder) return Path.GetDirectoryName(Application.dataPath);

            return "";
        }

        private void OnValidate()
        {
            if (mainCam == null)
            {
                mainCam = GetComponent<Camera>();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Helps icon creator saving the last instantiated prefab
        /// </summary>
        public static void SetInstantiatedItem(Scene scene, GameObject item)
        {
            EditorPrefs.SetString($"iconCreator{scene.name}", item.name);
        }

        /// <summary>
        /// Helps icon creator retrive the last instantiated prefab
        /// </summary>
        public GameObject GetInstantiatedItem(Scene scene)
        {

            if (!EditorPrefs.HasKey($"iconCreator{scene.name}"))
            {
                return null;
            }

            try
            { 
                GameObject finded = FindObjectsOfType<GameObject>().First( e => e.name == EditorPrefs.GetString($"iconCreator{scene.name}"));
               
                return finded;
            }
            catch
            {
                return null;
            }

             
        }
#endif
    }
}