using UnityEngine;
using UnityEngine.UI;

namespace IconCreator
{
    public class IconCreatorCanvas : MonoBehaviour
    {
        public Text textLabel;
        public GameObject borders;
        public Button BuildIconsBtn;

        public static IconCreatorCanvas instance;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            textLabel.text = "Icon creator";
        }

        public void SetInfo(int totalItens, int currentItem, string itemName, bool isRecording, string nextIconKey, string forceIconKeyMain, string nextIconNew)
        {
            borders.gameObject.SetActive(isRecording);

            if (isRecording == false)
            {
                BuildIconsBtn.gameObject.SetActive(true);
                textLabel.text = "Icon creator";
                return;
            }

            if (string.IsNullOrEmpty(nextIconNew))
            {
                textLabel.text = currentItem + " / " + totalItens + " - " + itemName + "   |   Press <b>" + nextIconKey + "</b> to continue \nPress <b>" + forceIconKeyMain + "</b> to force creating an icon";
            } else
            {
                textLabel.text = currentItem + " / " + totalItens + " - " + itemName + "   |   Press <b>" + nextIconKey + $"</b> or <b>{nextIconNew.ToString()}</b> to continue \nPress <b>" + forceIconKeyMain + "</b> to force creating an icon";
            } 
        }

        /// <summary>
        /// Starts build icons, called on onClick
        /// </summary>
        public void BuildIconsClick()
        {
            PrefabIconCreator PrefabIconCreator = GameObject.FindObjectOfType<PrefabIconCreator>();
            if (!System.Object.ReferenceEquals(PrefabIconCreator, null))
            {
                PrefabIconCreator.BuildIcons();
                return;
            }

            MaterialIconCreator materialIconCreator = GameObject.FindObjectOfType<MaterialIconCreator>();
            if (!System.Object.ReferenceEquals(materialIconCreator, null))
            {
                materialIconCreator.BuildIcons();
            }
        }

        public void SetTakingPicture()
        {
            textLabel.text = "Generating icon...";
        }
    }
}