using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;

    [SerializeField] private LanguageManager languageManager;

    public void ShowInfo(IconInfo iconInfo)
    {
        if(iconInfo.icon != null)
        {
            icon.sprite = iconInfo.icon;
        }
        if(iconInfo.nameIcon != "")
        {
            if(languageManager.currentLanguage == Language.Russian)
            {
                nameText.text = iconInfo.nameIcon;
            }
            else if(languageManager.currentLanguage == Language.English)
            {
                nameText.text = iconInfo.engNameIcon;
            }
            else if (languageManager.currentLanguage == Language.Indonesian)
            {
                nameText.text = iconInfo.indonesianNameIcon;
            }
        }
        if(iconInfo.descriptionIcon != "")
        {
            if (languageManager.currentLanguage == Language.Russian)
            {
                descriptionText.text = iconInfo.descriptionIcon;
            }
            else if (languageManager.currentLanguage == Language.English)
            {
                descriptionText.text = iconInfo.engDescriptionIcon;
            }
            else if (languageManager.currentLanguage == Language.Indonesian)
            {
                descriptionText.text = iconInfo.indonesianDescriptionIcon;
            }
        }
    }
}
