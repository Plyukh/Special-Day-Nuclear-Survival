using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField] Character character;

    [SerializeField] private Text strengthText;
    [SerializeField] private Text agilityText;
    [SerializeField] private Text intelligenceText;
    [SerializeField] private Text charismaText;

    [SerializeField] private Text pointsText;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private Text hpText;
    [SerializeField] private Text xpText;
    [SerializeField] private Text lvlText;

    [SerializeField] private Button closeButton;

    [SerializeField] private Text[] skillsTexts;

    [SerializeField] private LanguageManager languageManager;

    private void Update()
    {
        strengthText.text = character.Attributes.Strength.ToString() + "/10";
        agilityText.text = character.Attributes.Agility.ToString() + "/10";
        intelligenceText.text = character.Attributes.Intelligence.ToString() + "/10";
        charismaText.text = character.Attributes.Charisma.ToString() + "/10";

        strengthText.color = SelectColor(character.Attributes.Strength, 10);
        agilityText.color = SelectColor(character.Attributes.Agility, 10);
        intelligenceText.color = SelectColor(character.Attributes.Intelligence, 10);
        charismaText.color = SelectColor(character.Attributes.Charisma, 10);

        if(languageManager.currentLanguage == Language.Russian)
        {
            pointsText.text = "Нераспределенные очки: " + character.Attributes.points.ToString();
        }
        else if (languageManager.currentLanguage == Language.English)
        {
            pointsText.text = "Unallocated points: " + character.Attributes.points.ToString();
        }
        else if (languageManager.currentLanguage == Language.Indonesian)
        {
            pointsText.text = "Poin yang belum dialokasikan: " + character.Attributes.points.ToString();
        }

        for (int i = 0; i < skillsTexts.Length; i++)
        {
            skillsTexts[i].text = character.CharacterSkills[i].points.ToString() + "/100";
            skillsTexts[i].color = SelectColor(character.CharacterSkills[i].points, 100);
        }

        if(hpSlider != null)
        {
            hpText.text = character.GetComponent<HealthSystem>().health + "/" + character.GetComponent<HealthSystem>().maxHealth;

            hpSlider.value = character.GetComponent<HealthSystem>().health;
            hpSlider.maxValue = character.GetComponent<HealthSystem>().maxHealth;
        }
        if(xpSlider != null)
        {
            xpText.text = ExperienceSystem.currentXP + "/" + ExperienceSystem.nextLvlXP;

            xpSlider.value = ExperienceSystem.currentXP;
            xpSlider.maxValue = ExperienceSystem.nextLvlXP;

            if(languageManager.currentLanguage == Language.Russian)
            {
                lvlText.text = "Текущий уровень\n" + ExperienceSystem.lvl.ToString();
            }
            else if (languageManager.currentLanguage == Language.English)
            {
                lvlText.text = "Current level\n" + ExperienceSystem.lvl.ToString();
            }
            else if (languageManager.currentLanguage == Language.Indonesian)
            {
                lvlText.text = "Tingkat saat ini\n" + ExperienceSystem.lvl.ToString();
            }
        }

        if(closeButton != null)
        {
            if(character.Attributes.points > 0)
            {
                closeButton.interactable = false;
            }
            else
            {
                closeButton.interactable = true;
            }
        }
    }

    private Color32 SelectColor(int value, int maxvalue)
    {
        if(maxvalue == 100)
        {
            if (value < 26)
            {
                return Color.red;
            }
            else if (value > 25 && value < 51)
            {
                return new Color32(255, 165, 0, 255);
            }
            else if (value > 50 && value < 76)
            {
                return Color.yellow;
            }
            else if (value > 75)
            {
                return Color.green;
            }
            else
            {
                return Color.white;
            }
        }
        else if(maxvalue == 10)
        {
            if (value < 3)
            {
                return Color.red;
            }
            else if (value > 2 && value < 5)
            {
                return new Color32(255, 165, 0, 255);
            }
            else if (value > 4 && value < 7)
            {
                return Color.yellow;
            }
            else if (value > 6)
            {
                return Color.green;
            }
            else
            {
                return Color.white;
            }
        }
        else
        {
            return Color.white;
        }
    }
}