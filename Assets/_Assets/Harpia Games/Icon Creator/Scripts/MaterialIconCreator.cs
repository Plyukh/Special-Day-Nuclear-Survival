using System.Collections;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
//If there's an error here, remember to add input system to assembly definition
//Go to to Documentation -> Using the new input system 
//https://docs.google.com/document/d/1O7FnBUAFJEZwadJSbIgfp5peQOi2QJfD_77_FMJ_i8g/
using UnityEngine.InputSystem;
#endif

namespace IconCreator
{
    [ExecuteInEditMode]
    public class MaterialIconCreator : IconCreator
    {
        public Renderer targetRenderer;
        public Material[] materials;

        public override void BuildIcons()
        {
            StartCoroutine(BuildIconsRotine());
        }

        public override bool CheckConditions()
        {
            if (base.CheckConditions() == false) return false;

            if (materials.Length == 0)
            {
                Debug.LogError("There's no materials");
                return false;
            }

            if (targetRenderer == null)
            {
                Debug.LogError("There's no target renderer");
                return false;
            }

            return true;
        }

        private IEnumerator BuildIconsRotine()
        {
            Initialize();

            if (dynamicFov) UpdateFOV(targetRenderer.gameObject);
            if (lookAtObjectCenter) LookAtTargetCenter(targetRenderer.gameObject);

            currentObject = targetRenderer.transform;

            yield return CreateIconRotine(targetRenderer.name, 0);

            for (int i = 0; i < materials.Length; i++)
            {
                if (System.Object.ReferenceEquals(materials[i], null))
                {
                    continue;
                }

                
                targetRenderer.material = materials[i];
                targetRenderer.materials[0] = materials[i];
                currentObjectName = materials[i].name;


#if ENABLE_LEGACY_INPUT_MANAGER && ENABLE_INPUT_SYSTEM
                if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(materials.Length, i, materials[i].name, true, nextIconKeyLegacy.ToString(), forceIconLegacy.ToString(), nextIconKey.ToString());
#elif ENABLE_LEGACY_INPUT_MANAGER
                if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(materials.Length, i, materials[i].name, true, nextIconKeyLegacy.ToString(), forceIconLegacy.ToString(),"");
#elif ENABLE_INPUT_SYSTEM
                if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(materials.Length, i, materials[i].name, true, nextIconKey.ToString(), forceIcon.ToString(),"");
#endif


                if (whiteCam != null) whiteCam.enabled = false;
                if (whiteCam != null) blackCam.enabled = false;

                if (mode == Mode.Manual)
                {
                    CanMove = true;
#if ENABLE_LEGACY_INPUT_MANAGER && ENABLE_INPUT_SYSTEM
                    yield return new WaitUntil(() => Input.GetKeyDown(nextIconKeyLegacy) || Keyboard.current[nextIconKey].wasPressedThisFrame);
#elif ENABLE_LEGACY_INPUT_MANAGER
                    yield return new WaitUntil(() => Input.GetKeyDown(nextIconKeyLegacy));
#elif ENABLE_INPUT_SYSTEM
                    yield return new WaitUntil(() => Keyboard.current[nextIconKey].wasPressedThisFrame);
#endif
                    CanMove = false;
                }

                if (IconCreatorCanvas.instance != null)
                {
                    IconCreatorCanvas.instance.SetTakingPicture();
                    yield return null;
                    yield return null;
                    yield return null;
                }
                yield return CreateIconRotine(materials[i].name, i);
            }

#if ENABLE_LEGACY_INPUT_MANAGER && ENABLE_INPUT_SYSTEM
            if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(0, 0, "", false, nextIconKeyLegacy.ToString(), forceIconLegacy.ToString(), nextIconKey.ToString());
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(0, 0, "", false, nextIconKeyLegacy.ToString(), forceIconLegacy.ToString(), "");
#elif ENABLE_INPUT_SYSTEM
            if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(0, 0, "", false, nextIconKey.ToString(),forceIcon.ToString(), "");
#endif
            RevealInFinder();
            DeleteCameras();
        }

        private void Reset()
        {
            targetRenderer = null;
            materials = new Material[0];
        }

        protected override void Update()
        {
            if (preview && !isCreatingIcons)
            {
                if (targetRenderer != null)
                {
                    if (dynamicFov) UpdateFOV(targetRenderer.gameObject);
                    if (lookAtObjectCenter) LookAtTargetCenter(targetRenderer.gameObject);
                }
                return;
            }

            base.Update();
        }
    }
}