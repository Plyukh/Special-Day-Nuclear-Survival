using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageComponent : MonoBehaviour
{
    Text LanguageText;
    [SerializeField] string ruText;
    [SerializeField] string engText;
    [SerializeField] string indonesianText;

    public void SetLanguageText(Language language)
    {
        if(LanguageText == null)
        {
            LanguageText = GetComponent<Text>();
        }

        if(language == Language.Russian)
        {
            LanguageText.text = ruText;
        }
        else if(language == Language.English)
        {
            LanguageText.text = engText;
        }
        else if (language == Language.Indonesian)
        {
            LanguageText.text = indonesianText;
        }
    }
}
