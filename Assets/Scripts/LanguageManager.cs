using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Language
{
    Russian,
    English,
    Indonesian
}

public class LanguageManager : MonoBehaviour
{
    public Language currentLanguage;

    [SerializeField] List<LanguageComponent> languageComponents;

    public void SetLanguage(Language language)
    {
        currentLanguage = language;

        foreach (var item in languageComponents)
        {
            item.SetLanguageText(currentLanguage);
        }
    }
    public void SetLanguage(int languageIndex)
    {
        currentLanguage = (Language)languageIndex;

        foreach (var item in languageComponents)
        {
            item.SetLanguageText(currentLanguage);
        }
    }
}
