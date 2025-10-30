using UnityEngine;
using UnityEngine.UI;

public class Blueprint : MonoBehaviour
{
    [SerializeField] private LanguageManager languageManager;
    public Item blueprintItem;

    public int needScienceSkill;
    public Item[] needItems;

    private void Start()
    {
        transform.GetChild(1).GetComponent<Image>().sprite = blueprintItem.itemSprite;
        transform.GetChild(2).GetComponent<Image>().sprite = blueprintItem.itemTypeSprite;
        if (languageManager.currentLanguage == Language.Russian)
        {
            transform.GetChild(1).GetChild(0).GetComponent<Text>().text = blueprintItem.itemName;
        }
        else if (languageManager.currentLanguage == Language.English)
        {
            transform.GetChild(1).GetChild(0).GetComponent<Text>().text = blueprintItem.englishItemName;
        }
        else if (languageManager.currentLanguage == Language.Indonesian)
        {
            transform.GetChild(1).GetChild(0).GetComponent<Text>().text = blueprintItem.indonesianItemName;
        }

        if (blueprintItem.stack)
        {
            transform.GetChild(3).GetComponent<Text>().text = blueprintItem.number.ToString();
        }
        else
        {
            transform.GetChild(3).GetComponent<Text>().text = "";
        }
    }

    public void UpdateNumberText()
    {
        int newAmmo = Mathf.RoundToInt(blueprintItem.number / 4 + blueprintItem.number);
        transform.GetChild(3).GetComponent<Text>().text = newAmmo.ToString();
    }
}