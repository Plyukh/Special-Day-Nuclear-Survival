using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
//If there's an error here, remember to add input system to assembly definition
//Go to to Documentation -> Using the new input system 
//https://docs.google.com/document/d/1O7FnBUAFJEZwadJSbIgfp5peQOi2QJfD_77_FMJ_i8g/
using UnityEngine.InputSystem;
#endif

namespace IconCreator
{
    [ExecuteInEditMode]
    [SelectionBase]
    public class PrefabIconCreator : IconCreator
    {
        [Header("Itens")]
        public GameObject[] itensToShot;
        private GameObject instantiatedItem;

        /// <summary>
        /// Starts building icons
        /// </summary>
        public override void BuildIcons()
        {
            if(itensToShot.Length == 0)
            {
                Debug.LogError($"(Icon Creator) There's no itens to shot. Please fill the itens to shot prefab list",this);
                return;
            }
            StartCoroutine(BuildAllIcons());
        }

        /// <summary>
        /// Verify if can start building icons
        /// </summary>
        /// <returns></returns>
        public override bool CheckConditions()
        {
            if (base.CheckConditions() == false) return false;

            if (itensToShot.Length == 0)
            {
                Debug.LogError("There's no prefab to shoot");
                return false;
            }

            return true;
        }

#if UNITY_EDITOR

        protected override void Update()
        {
            if (!preview && !isCreatingIcons) return;


            if (instantiatedItem != null)
            {
                if (dynamicFov) UpdateFOV(instantiatedItem);
                if (lookAtObjectCenter) LookAtTargetCenter(instantiatedItem);

            }
            

            base.Update();
        }

        public void InstantiateItem()
        {
            if (itensToShot[0] != null)
            {
                instantiatedItem = Instantiate(itensToShot[0], transform.position + transform.forward * 3, Quaternion.identity, null);
                instantiatedItem.name += " (Icon creator item)";
                instantiatedItem.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
                SetInstantiatedItem(SceneManager.GetActiveScene(), instantiatedItem);
                Debug.Log($"Icon creator instantiated {instantiatedItem.name}. If you do not want this behaviour, disable 'Preview in edit mode' option", this);
            }
        }



        private void OnValidate()
        {
            DestroyAllIconCreatorObjects();
        }

        /// <summary>
        /// Destroy item instance
        /// </summary>
        protected override void DestroyInstances()
        {
            if (!System.Object.ReferenceEquals(instantiatedItem, null))
            {
                if (!Application.isPlaying)
                {
                    StartCoroutine(base.Destroy(instantiatedItem));
                }
                else
                {
                    Destroy(instantiatedItem);

                }
            }
            instantiatedItem = null;
        }

#endif





        public IEnumerator BuildAllIcons()
        {
            Initialize();

            for (int i = 0; i < itensToShot.Length; i++)
            {
                if (System.Object.ReferenceEquals(itensToShot[i], null)) continue;

                DestroyAllIconCreatorObjects();
                Vector3 pos = transform.position + transform.forward * 10;

                if (instantiatedItem != null)
                {
                    pos = instantiatedItem.transform.position;
                    DestroyImmediate(instantiatedItem);
                }

                if (whiteCam != null) whiteCam.enabled = false;
                if (blackCam != null) blackCam.enabled = false;

                instantiatedItem = Instantiate(itensToShot[i], pos, Quaternion.identity, null);
                currentObjectName = instantiatedItem.name;


#if ENABLE_LEGACY_INPUT_MANAGER && ENABLE_INPUT_SYSTEM
                if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(itensToShot.Length, i, itensToShot[i].name, true, nextIconKeyLegacy.ToString(), forceIconLegacy.ToString(), nextIconKey.ToString());

#elif ENABLE_LEGACY_INPUT_MANAGER
                if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(itensToShot.Length, i, itensToShot[i].name, true, nextIconKeyLegacy.ToString(), forceIconLegacy.ToString(), "");

#elif ENABLE_INPUT_SYSTEM
                 if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(itensToShot.Length, i, itensToShot[i].name, true, nextIconKey.ToString(), forceIcon.ToString(), "");
#endif




#if UNITY_EDITOR
                if (IconCreatorAnimations.instance != null) IconCreatorAnimations.instance.SetAnimationObject(instantiatedItem);
#endif

                currentObject = instantiatedItem.transform;

                if (dynamicFov) UpdateFOV(instantiatedItem);
                if (lookAtObjectCenter) LookAtTargetCenter(instantiatedItem);

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
                }

                yield return CreateIconRotine(itensToShot[i].name, i);
            }


#if ENABLE_LEGACY_INPUT_MANAGER && ENABLE_INPUT_SYSTEM
            if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(0, 0, "", false, nextIconKeyLegacy.ToString(), forceIconLegacy.ToString(), nextIconKey.ToString());
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(0, 0, "", false, nextIconKeyLegacy.ToString(), forceIconLegacy.ToString(), "");
#elif ENABLE_INPUT_SYSTEM
            if (IconCreatorCanvas.instance != null) IconCreatorCanvas.instance.SetInfo(0, 0, "", false, nextIconKey.ToString(),forceIcon.ToString(),"");
#endif

            RevealInFinder();

            DeleteCameras();
        }



    }


}