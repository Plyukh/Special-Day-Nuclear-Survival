using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IconCreator
{

    public class IconCreatorUIAux : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// The main icon creator object
        /// </summary>
        IconCreator creator;
        
        /// <summary>
        /// If the user is pressing the UI
        /// </summary>
        [System.NonSerialized]
        bool isPressing = false;

        void Start()
        {
            creator = GameObject.FindObjectOfType<IconCreator>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            creator.SetCanRotate(false);
            isPressing = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isPressing) return;
            creator.SetCanRotate(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressing = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressing = false;
            creator.SetCanRotate(true);
        }

        
    }
}