using UnityEngine;
using UnityEngine.UI;

public class PerkSystem : MonoBehaviour
{
    [SerializeField] private Character player;

    public int points;
    [SerializeField] private Text pointText;

    public Perk[] perks;

    [SerializeField] private Slider[] sliders;
    [SerializeField] private Text[] skillTexts;

    private float needTime = 0.05f;
    private float currentTime = 0;

    [SerializeField] private Image infoImage;
    [SerializeField] private Image infoPerkImage;
    [SerializeField] private Text infoName;
    [SerializeField] private Text infoNumber;
    [SerializeField] private Text infoDescription;
    [SerializeField] private Button perkButton;

    private Perk currentPerk;
    private LanguageManager languageManager;

    private void Start()
    {
        if (languageManager == null)
        {
            languageManager = GetComponent<HealthSystem>().questSystem.languageManager;
        }
    }

    public void UpdateStats()
    {
        if (languageManager.currentLanguage == Language.Russian)
        {
            pointText.text = "Нераспределенные очки перков: " + points.ToString();
        }
        else if (languageManager.currentLanguage == Language.English)
        {
            pointText.text = "Unallocated perk points: " + points.ToString();
        }
        else if (languageManager.currentLanguage == Language.Indonesian)
        {
            pointText.text = "Poin keistimewaan yang belum dialokasikan: " + points.ToString();
        }

        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].maxValue = 100;
            sliders[i].value = 0;
        }

        for (int i = 0; i < skillTexts.Length; i++)
        {
            skillTexts[i].text = player.CharacterSkills[i].points + "/" + 100;
        }

        for (int i = 0; i < perks.Length; i++)
        {
            perks[i].SelectColor(Color.gray);
        }
    }

    private void Update()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            if(sliders[i].value < player.CharacterSkills[i].points)
            {
                currentTime += Time.deltaTime;
                if(currentTime >= needTime)
                {
                    UpdateSliders();
                    currentTime = 0;
                }
            }
        }
    }

    public void UpdateSliders()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            if(sliders[i].value < player.CharacterSkills[i].points)
            {
                sliders[i].value += 1;
                for (int j = 0; j < perks.Length; j++)
                {
                    perks[j].UpdatePerk(player.CharacterSkills[i].skill, sliders[i].value);
                }
            }
        }
    }
    public void CheckUnallocatedPerks()
    {
        if (points > 0)
        {
            ExperienceSystem.ActiveLvlSmallIcon();
        }
    }

    public Perk FindPerk(Skills skill, int id)
    {
        for (int i = 0; i < perks.Length; i++)
        {
            if(perks[i].skill == skill)
            {
                if(perks[i].id == id)
                {
                    return perks[i];
                }
            }
        }
        return null;
    }

    public void ShowPerkInfo(Perk perk)
    {
        currentPerk = perk;

        bool lvl = false;

        if(player.Attributes.points > 0)
        {
            lvl = true;
        }
        if(lvl == false)
        {
            for (int i = 0; i < player.CharacterSkills.Length; i++)
            {
                if (player.CharacterSkills[i].newPoints != 0)
                {
                    lvl = true;
                    break;
                }
            }
        }

        if (points > 0 && !perk.Active && perk.transform.GetChild(1).GetComponent<Image>().color == Color.white && !lvl)
        {
            perkButton.interactable = true;
            perkButton.transform.GetChild(1).GetComponent<Text>().color = Color.white;
        }
        else
        {
            perkButton.interactable = false;
            perkButton.transform.GetChild(1).GetComponent<Text>().color = Color.gray;
        }

        infoImage.sprite = perk.transform.GetChild(1).GetComponent<Image>().sprite;
        infoPerkImage.sprite = perk.perkSprite;
        infoImage.rectTransform.localScale = perk.transform.GetChild(1).GetComponent<RectTransform>().localScale;

        if(languageManager.currentLanguage == Language.Russian)
        {
            infoName.text = perk.perkName;
            infoDescription.text = perk.perkDescription;
        }
        else if(languageManager.currentLanguage == Language.English)
        {
            infoName.text = perk.engPerkName;
            infoDescription.text = perk.engPerkDescription;
        }
        else if (languageManager.currentLanguage == Language.Indonesian)
        {
            infoName.text = perk.indonesianPerkName;
            infoDescription.text = perk.indonesianPerkDescription;
        }

        if (perk.Active)
        {
            infoImage.color = Color.yellow;
            infoNumber.text = "1/1";
            infoNumber.color = Color.yellow;
        }
        else
        {
            infoImage.color = Color.white;
            infoNumber.text = "0/1";
            infoNumber.color = Color.white;
        }
    }

    public void TakePerk()
    {
        points -= 1;

        currentPerk.Active = true;
        currentPerk.UpdatePerk();
        if((currentPerk.skill == Skills.Unarmed && currentPerk.id == 1) || (currentPerk.skill == Skills.Doctor && currentPerk.id == 1))
        {
            GetComponent<HealthSystem>().UpdatePerks(currentPerk.skill);
        }
        if (currentPerk.skill == Skills.Doctor && currentPerk.id == 0)
        {
            GetComponent<HealthSystem>().UpdateMaxHealth();
        }
        ShowPerkInfo(currentPerk);

        if (languageManager.currentLanguage == Language.Russian)
        {
            pointText.text = "Нераспределенные очки перков: " + points.ToString();
        }
        else if (languageManager.currentLanguage == Language.English)
        {
            pointText.text = "Unallocated perk points: " + points.ToString();
        }
        else if (languageManager.currentLanguage == Language.Indonesian)
        {
            pointText.text = "Poin keistimewaan yang belum dialokasikan: " + points.ToString();
        }
    }
}