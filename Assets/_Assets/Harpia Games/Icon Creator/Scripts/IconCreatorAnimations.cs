using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor.Animations;

#endif
namespace IconCreator
{
    public class IconCreatorAnimations : MonoBehaviour
    {
        /// <summary>
        /// The object animator
        /// </summary>
        [SerializeField]
        Animator animator;

        /// <summary>
        /// A list with all animations states
        /// </summary>
        List<Dropdown.OptionData> states;

        [Tooltip("Dropdown with all states")]
        public Dropdown dropdown;

        [Tooltip("Animation time slider")]
        public Slider sliderTime;

        /// <summary>
        /// A list with all the layers
        /// </summary>
        List<int> layers;

        /// <summary>
        /// The current animaiton
        /// </summary>
        string currentAnimation;

        public static IconCreatorAnimations instance;

        private void Awake()
        {
            instance = this;
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        /// <summary>
        /// SetUp the UI for the object, if it contains an animator component
        /// </summary>
        /// <param name="target">The target object</param>
        public void SetAnimationObject(GameObject target)
        {
            animator = FindAnimator(target);

            if (animator == null)
            {
                SetUIActive(false);
                return;
            }

            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogWarning("Theres no animator controller in " + target.gameObject.name);
                SetUIActive(false);
                return;
            }

            SetUIActive(true);

            currentAnimation = "";
            states = new List<Dropdown.OptionData>();
            dropdown.ClearOptions();

            foreach (AnimatorControllerLayer layer in ((AnimatorController)animator.runtimeAnimatorController).layers)
            {
                foreach (ChildAnimatorState state in layer.stateMachine.states)
                {
                    states.Add(new Dropdown.OptionData(state.state.name));
                    if (currentAnimation == "")
                    {
                        currentAnimation = state.state.name;
                    }
                }
            }

            if (states.Count == 0)
            {
                SetUIActive(false);
                Debug.LogWarning("Theres no animation states in " + target.gameObject.name);
                return;
            }

            dropdown.AddOptions(states);

            OnDropdownChange(0);

            gameObject.SetActive(true);
            animator.speed = 0;
        }
#endif



        private void SetUIActive(bool n)
        {
            dropdown.gameObject.SetActive(n);
            sliderTime.gameObject.SetActive(n);
        }

        /// <summary>
        /// Find an animator on the object and it's children
        /// </summary>
        /// <param name="target">The target object</param>
        /// <returns></returns>
        Animator FindAnimator(GameObject target)
        {
            if (target.GetComponent<Animator>())
            {
                return target.GetComponent<Animator>();
            }

            if (target.GetComponentInChildren<Animator>())
            {
                return target.GetComponentInChildren<Animator>();
            }


            return null;
        }

        /// <summary>
        /// Changes the animation state from the object's animator 
        /// </summary>
        /// <param name="index">The new animation state index</param>
        public void OnDropdownChange(int index)
        {


            currentAnimation = dropdown.options[index].text;
            animator.Play(currentAnimation);
            sliderTime.value = 0;
            animator.speed = 0;
            animator.enabled = true;
        }

        public void SetTime(float i)
        {
            if (animator == null) return;
            animator.Play(currentAnimation, -1, i);
            animator.speed = 0;

        }

    }
}