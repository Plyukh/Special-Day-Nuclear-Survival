using UnityEngine;
using UnityEngine.UI;

public enum QuestType
{
    Dialogue,
    Item,
    Murder,
}

public class QuestSystem : MonoBehaviour
{
    private Character player;
    public LanguageManager languageManager;

    [SerializeField] private Vector2 gridStartPosition;
    [SerializeField] private Vector2 gridStartSize;

    [SerializeField] private Vector2 gridAddPosition;
    [SerializeField] private Vector2 gridAddSize;

    [SerializeField] private GameObject activeQuestIcon;
    [SerializeField] private GameObject questBigIcon;
    [SerializeField] private RectTransform questGrid;
    [SerializeField] private GameObject questPartGrid;
    [SerializeField] private Button quest;
    [SerializeField] private GameObject questPart;

    [SerializeField] private Text questName;
    [SerializeField] private Text questDescription;

    [SerializeField] private Color32 completeColor, activeColor;

    public Quest[] quests;

    public Character Player
    {
        get
        {
            return player;
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
    }

    public void ShowQuests()
    {
        Camera.main.GetComponent<CameraZoom>().OnPointerObject();

        questGrid.sizeDelta = gridStartSize;
        questGrid.anchoredPosition = gridStartPosition;

        int x = questGrid.transform.childCount;
        bool first = true;

        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i].Active && !quests[i].Complete)
            {
                SpawnQuest(quests[i], activeColor, x);

                if (first)
                {
                    ShowQuestDescription(quests[i]);
                    first = false;
                }
            }
        }
        for (int i = 0; i < quests.Length; i++)
        {
            if (quests[i].Active && quests[i].Complete)
            {
                SpawnQuest(quests[i], completeColor, x);
                if (first)
                {
                    ShowQuestDescription(quests[i]);
                    first = false;
                }
            }
        }

        for (int i = 0; i < x; i++)
        {
            Destroy(questGrid.transform.GetChild(i).gameObject);
        }
    }

    void SpawnQuest(Quest quest, Color color, int x)
    {
        Button questObject = Instantiate(this.quest, questGrid);
        questObject.transform.GetChild(0).GetComponent<Image>().color = color;
        questObject.transform.GetChild(1).GetComponent<Text>().color = color;

        if(languageManager.currentLanguage == Language.Russian)
        {
            questObject.transform.GetChild(1).GetComponent<Text>().text = quest.Name;
        }
        else if(languageManager.currentLanguage == Language.English)
        {
            questObject.transform.GetChild(1).GetComponent<Text>().text = quest.EnglishName;
        }
        else if (languageManager.currentLanguage == Language.Indonesian)
        {
            questObject.transform.GetChild(1).GetComponent<Text>().text = quest.IndonesianName;
        }

        questObject.onClick.AddListener(() => AddSound());
        questObject.onClick.AddListener(() => ShowQuestDescription(quest));

        if (questGrid.transform.childCount > 6 + x)
        {
            questGrid.sizeDelta += gridAddSize;
            questGrid.anchoredPosition += gridAddPosition;
        }
    }

    void AddSound()
    {
        transform.parent.GetComponent<AudioSource>().Play();
    }
    void ShowQuestDescription(Quest quest)
    {
        if(languageManager.currentLanguage == Language.Russian)
        {
            questName.text = quest.Name;
            questDescription.text = quest.Description;
        }
        else if(languageManager.currentLanguage == Language.English)
        {
            questName.text = quest.EnglishName;
            questDescription.text = quest.EnglishDescription;
        }
        else if (languageManager.currentLanguage == Language.Indonesian)
        {
            questName.text = quest.IndonesianName;
            questDescription.text = quest.IndonesianDescription;
        }

        for (int i = 0; i < questPartGrid.transform.childCount; i++)
        {
            Destroy(questPartGrid.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < quest.QuestParts.Length; i++)
        {
            if (quest.QuestParts[i].Active)
            {
                GameObject questPart = Instantiate(this.questPart, questPartGrid.transform);

                if (languageManager.currentLanguage == Language.Russian)
                {
                    questPart.transform.GetChild(1).GetComponent<Text>().text = quest.QuestParts[i].Description;
                }
                else if (languageManager.currentLanguage == Language.English)
                {
                    questPart.transform.GetChild(1).GetComponent<Text>().text = quest.QuestParts[i].EnglishDescription;
                }
                else if (languageManager.currentLanguage == Language.Indonesian)
                {
                    questPart.transform.GetChild(1).GetComponent<Text>().text = quest.QuestParts[i].IndonesianDescription;
                }

                if (quest.QuestParts[i].Complete)
                {
                    questPart.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }

    public void ActiveQuest(int index)
    {
        activeQuestIcon.SetActive(true);
        questBigIcon.SetActive(false);
        questBigIcon.SetActive(true);

        quests[index].Active = true;
        if (quests[index].CompleteAllParts)
        {
            for (int i = 0; i < quests[index].QuestParts.Length; i++)
            {
                quests[index].QuestParts[i].Active = true;
            }
        }
        else
        {
            quests[index].QuestParts[0].Active = true;
        }
    }

    public void CompleteQuest(int index)
    {
        bool CompleteFirst = false;
        quests[index].Active = true;

        if (quests[index].Complete == false)
        {
            quests[index].Complete = true;
            CompleteFirst = true;
        }

        if(quests[index].CompleteNextQuest > -1)
        {
            CompletePart(quests[index].CompleteNextQuest, quests[index].CompleteNextQuestPart);
        }

        if (quests[index].CompleteAllParts)
        {
            for (int i = 0; i < quests[index].QuestParts.Length; i++)
            {
                quests[index].QuestParts[i].Active = true;
            }
        }

        if (CompleteFirst)
        {
            ExperienceSystem.AddXP(quests[index].XP);
            if (quests[index].AchivementID > -1)
            {
                //googlePlayAchievements.UnlockAchievement(quests[index].AchivementID);
            }
        }
    }
    public void CompletePart(int QuestIndex, int PartIndex)
    {
        quests[QuestIndex].QuestParts[PartIndex].Complete = true;

        if (quests[QuestIndex].CompleteAllParts)
        {
            bool completeAll = true;

            for (int i = 0; i < quests[QuestIndex].QuestParts.Length; i++)
            {
                if(quests[QuestIndex].QuestParts[i].Complete == false)
                {
                    quests[QuestIndex].QuestParts[i].Active = true;
                    completeAll = false;
                    break;
                }
            }

            if (completeAll)
            {
                if(quests[QuestIndex].Complete == false)
                {
                    CompleteQuest(QuestIndex);
                }
            }
        }
        else
        {
            if (quests[QuestIndex].QuestParts[PartIndex].nextPart != -1)
            {
                quests[QuestIndex].QuestParts[quests[QuestIndex].QuestParts[PartIndex].nextPart].Active = true;
            }
            else
            {
                CompleteQuest(QuestIndex);
            }
        }

        if (quests[QuestIndex].Active)
        {
            activeQuestIcon.SetActive(true);
        }
    }
}

[System.Serializable]
public class Quest
{
    public string Name;
    public string EnglishName;
    public string IndonesianName;
    public string Description;
    public string EnglishDescription;
    public string IndonesianDescription;

    public int XP;

    public QuestPart[] QuestParts;
    public bool CompleteAllParts;

    public int CompleteNextQuest;
    public int CompleteNextQuestPart;

    public bool Complete;
    public bool Active;

    public int AchivementID = -1;
}

[System.Serializable]
public class QuestPart
{
    public string Description;
    public string EnglishDescription;
    public string IndonesianDescription;
    public QuestType QuestType;
    public Item Item;
    public string characterName;

    public bool Complete;
    public bool Active;

    public int nextPart;
}