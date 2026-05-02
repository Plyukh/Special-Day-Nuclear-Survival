using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    const float FogRevealRadius = 175f;
    const float CellArrivalDistSqr = 1f;

    float fogRevealRadiusSqr;

    [SerializeField] private LanguageManager languageManager;
    [SerializeField] private Settings settings;
    [SerializeField] private Animator infoObject;
    [SerializeField] private Text nameLocation;
    [SerializeField] private Text descriptionLocation;
    [SerializeField] private Image imageLocation;
    [SerializeField] private Button locationsButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private GameObject playerPoint;
    [SerializeField] private Fog[] fog;
    bool canTravel;

    [HideInInspector] public Button currentCell;
    [HideInInspector] public Button lastCell;

    [HideInInspector] public Fog encounterCell;
    public float timeToEncounter;
    public float currentTimeToEncounter;

    [SerializeField] private string[] allRandomEncounters;
    public bool[] attendedRandomEncounter;

    public float speed;

    public GameObject PlayerMapPoint
    {
        get
        {
            return playerPoint;
        }
    }
    public Fog[] Fog
    {
        get
        {
            return fog;
        }
    }

    void Awake()
    {
        fogRevealRadiusSqr = FogRevealRadius * FogRevealRadius;
    }

    bool TryStartRandomEncounterAfterTravelTimer()
    {
        if (encounterCell == null || allRandomEncounters == null || encounterCell.randomEncounters == null)
            return false;

        string[] encounterRandom = encounterCell.randomEncounters;

        for (int i = 0; i < allRandomEncounters.Length; i++)
        {
            if (attendedRandomEncounter[i])
                continue;

            string allName = allRandomEncounters[i];
            for (int j = 0; j < encounterRandom.Length; j++)
            {
                if (encounterRandom[j] != allName)
                    continue;

                Fog currentFog = encounterCell;
                lastCell = currentFog.GetComponent<Button>();
                currentCell = null;

                RandomEncounter(currentFog);
                return true;
            }
        }

        return false;
    }

    private void Update()
    {
        if (!canTravel || currentCell == null)
            return;

        playerPoint.transform.position = Vector2.MoveTowards(
            (Vector2)playerPoint.transform.position,
            (Vector2)currentCell.transform.position,
            speed * Time.deltaTime);

        if (timeToEncounter > 0f)
        {
            currentTimeToEncounter += Time.deltaTime;
            if (currentTimeToEncounter >= timeToEncounter)
            {
                if (TryStartRandomEncounterAfterTravelTimer())
                    return;

                currentTimeToEncounter = 0f;
            }
        }

        if (fog != null)
        {
            for (int i = 0; i < fog.Length; i++)
            {
                Fog f = fog[i];
                if (f == null)
                    continue;

                Vector2 fogPos = f.transform.position;
                Vector2 pp = playerPoint.transform.position;
                if ((fogPos - pp).sqrMagnitude > fogRevealRadiusSqr)
                    continue;

                if (!f.find)
                {
                    f.find = true;
                    f.Find();
                }
            }
        }

        Fog cellFog = currentCell.GetComponent<Fog>();
        if (cellFog == null)
            return;

        Vector3 playerPos3 = playerPoint.transform.position;
        float distToCellSqr = (playerPos3 - currentCell.transform.position).sqrMagnitude;

        if (distToCellSqr <= CellArrivalDistSqr && cellFog.location.sceneName != "")
        {
            lastCell = currentCell;
            currentCell = null;
            infoObject.gameObject.SetActive(false);
            infoObject.gameObject.SetActive(true);
            if(languageManager.currentLanguage == Language.Russian)
            {
                nameLocation.text = cellFog.location.locationName;
                descriptionLocation.text = cellFog.location.description;
            }
            else if (languageManager.currentLanguage == Language.English)
            {
                nameLocation.text = cellFog.location.engLocationName;
                descriptionLocation.text = cellFog.location.engDescription;
            }
            else if (languageManager.currentLanguage == Language.Indonesian)
            {
                nameLocation.text = cellFog.location.indonesianLocationName;
                descriptionLocation.text = cellFog.location.indonesianDescription;
            }
            imageLocation.sprite = cellFog.location.spriteLocation;

            locationsButton.onClick.RemoveAllListeners();
            locationsButton.onClick.AddListener(() => AddSound());
            locationsButton.onClick.AddListener(() => SaveMap());
            locationsButton.onClick.AddListener(() => LoadScene(cellFog.location.sceneName));
        }
        else if (distToCellSqr <= CellArrivalDistSqr)
        {
            timeToEncounter = 0;
            currentTimeToEncounter = 0;
        }
    }

    private void RandomEncounter(Fog currentFog)
    {
        bool findAll = true;
        for (int i = 0; i < currentFog.randomEncounters.Length; i++)
        {
            if (findAll)
            {
                for (int j = 0; j < allRandomEncounters.Length; j++)
                {
                    if (currentFog.randomEncounters[i] == allRandomEncounters[j])
                    {
                        if (attendedRandomEncounter[j] == false)
                        {
                            findAll = false;
                            break;
                        }
                    }
                }
            }
        }
        if(findAll == false)
        {
            int random = Random.Range(0, encounterCell.randomEncounters.Length);

            for (int i = 0; i < allRandomEncounters.Length; i++)
            {
                if (currentFog.randomEncounters[random] == allRandomEncounters[i])
                {
                    if (attendedRandomEncounter[i] == true)
                    {
                        RandomEncounter(currentFog);
                        break;
                    }
                    else
                    {
                        attendedRandomEncounter[i] = true;

                        bool findAllRandomEncounters = true;
                        for (int j = 0; j < attendedRandomEncounter.Length; j++)
                        {
                            if (attendedRandomEncounter[j] == false)
                            {
                                findAllRandomEncounters = false;
                            }
                        }
                        if (findAllRandomEncounters)
                        {
                            //googlePlayAchievements.UnlockAchievement(19);
                        }
                        SaveMap();
                        LoadScene(currentFog.randomEncounters[random]);
                        return;
                    }
                }
            }
        }
    }

    public void UpdateFog()
    {
        foreach (var item in fog)
        {
            if (item.find)
            {
                item.Find();
            }
        }
    }

    public void OpenMap()
    {
        foreach (var item in fog)
        {
            item.find = true;
        }
    }

    public void CanTravel(bool value)
    {
        currentCell = null;

        canTravel = value;

        if (value)
        {
            closeButton.interactable = false;
            transform.parent.gameObject.SetActive(true);
            transform.parent.GetChild(0).GetComponent<PanelScript>().SelectWindow(1);
            transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(false);
            settings.saveScript.Save();

            UpdateFog();
        }

        if (transform.parent.parent.GetComponent<QuestSystem>().quests[13].Complete)
        {
            speed = 50;
        }
        else
        {
            speed = 25;
        }
    }

    public void SelectCell(Button button)
    {
        currentCell = button;

        GameObject point = Instantiate(pointPrefab, button.transform);
        Destroy(point, 1f);

        infoObject.SetTrigger("Hide");
    }

    void AddSound()
    {
        transform.parent.parent.parent.GetComponent<AudioSource>().Play();
    }
    void LoadScene(string value)
    {
        settings.saveScript.GetComponent<SceneManagerScript>().LoadScene(value);
    }
    void SaveMap()
    {
        settings.saveScript.SaveMap();
    }
}