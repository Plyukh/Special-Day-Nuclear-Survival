using UnityEngine;
using UnityEngine.UI;

public class ExperienceSystem : MonoBehaviour
{
    static private Character player;
    static private LanguageManager languageManager;

    static private AudioSource audioSource;

    static private GameObject lvlBigIcon;
    static private GameObject lvlSmallIcon;

    static private Slider xpSlider;

    static public float currentXP;
    static public float nextLvlXP;
    static public int lvl = 1;

    static private float xp = 0;

    public float GetCurrentXP
    {
        get
        {
            return currentXP;
        }
    }
    public int GetCurrentLVL
    {
        get
        {
            return lvl;
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        languageManager = FindFirstObjectByType<LanguageManager>();
        lvlBigIcon = GameObject.Find("New Quest - Level - Lock Picking").transform.GetChild(1).gameObject;
        lvlSmallIcon = GameObject.Find("Character Stats Button").transform.GetChild(0).gameObject;
        xpSlider = GetComponent<Slider>();
        audioSource = GetComponent<AudioSource>();
    }

    public static void AddXP(float value, bool print = true)
    {
        if (print)
        {
            if (languageManager.currentLanguage == Language.Russian)
            {
                EventLog.Print("Вы получили " + value + " очков опыта", Color.green);
            }
            else if(languageManager.currentLanguage == Language.English)
            {
                EventLog.Print("You have gained " + value + " experience points", Color.green);
            }
            else if (languageManager.currentLanguage == Language.Indonesian)
            {
                EventLog.Print("Kamu telah mendapatkan " + value + " poin pengalaman", Color.green);
            }
            audioSource.Play();
        }

        currentXP += value;
        xp = 0;

        if (currentXP >= nextLvlXP)
        {
            if(currentXP > nextLvlXP)
            {
                xp = currentXP - nextLvlXP;
            }
            currentXP = 0;
            lvl += 1;

            if(lvl % 2 == 0)
            {
                player.PerkSystem.points += 1;
            }

            player.Attributes.AddPoints();

            lvlBigIcon.SetActive(false);
            lvlBigIcon.SetActive(true);

            lvlSmallIcon.SetActive(true);

            player.GetComponent<HealthSystem>().UpdateMaxHealth();
        }

        UpdateSlider();

        if(xp != 0)
        {
            AddXP(xp, false);
        }
    }

    public static void UpdateSlider()
    {
        nextLvlXP = lvl * 1000;

        xpSlider.maxValue = nextLvlXP;
        xpSlider.value = currentXP;
        xpSlider.transform.GetChild(1).GetComponent<Text>().text = currentXP + "/" + nextLvlXP;
    }
    public static void ActiveLvlSmallIcon()
    {
        lvlSmallIcon.SetActive(true);
    }

    public void SetXP_LVL(float XP, int LVL)
    {
        currentXP = XP;
        lvl = LVL;
    }
}