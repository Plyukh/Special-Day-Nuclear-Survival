using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
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

    private void Update()
    {
        if (canTravel)
        {
            if(currentCell != null)
            {
                playerPoint.transform.position = Vector2.MoveTowards(playerPoint.transform.position, currentCell.transform.position, speed * Time.deltaTime);

                if(timeToEncounter > 0)
                {
                    currentTimeToEncounter += Time.deltaTime;
                    if(currentTimeToEncounter >= timeToEncounter)
                    {
                        for (int i = 0; i < allRandomEncounters.Length; i++)
                        {
                            for (int j = 0; j < encounterCell.randomEncounters.Length; j++)
                            {
                                if (encounterCell.randomEncounters[j] == allRandomEncounters[i])
                                {
                                    if (attendedRandomEncounter[i] == false)
                                    {
                                        Fog currentFog = encounterCell;
                                        lastCell = currentFog.GetComponent<Button>();
                                        currentCell = null;

                                        RandomEncounter(currentFog);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < fog.Length; i++)
                {
                    if(Vector2.Distance(fog[i].transform.position, playerPoint.transform.position) <= 175)
                    {
                        fog[i].find = true;
                        fog[i].Find();
                    }
                }

                if(Vector3.Distance(playerPoint.transform.position, currentCell.transform.position) <= 1 && currentCell.GetComponent<Fog>().location.sceneName != "")
                {
                    Fog currentFog = currentCell.GetComponent<Fog>();
                    lastCell = currentCell;
                    currentCell = null;
                    infoObject.gameObject.SetActive(false);
                    infoObject.gameObject.SetActive(true);
                    if(languageManager.currentLanguage == Language.Russian)
                    {
                        nameLocation.text = currentFog.location.locationName;
                        descriptionLocation.text = currentFog.location.description;
                    }
                    else if (languageManager.currentLanguage == Language.English)
                    {
                        nameLocation.text = currentFog.location.engLocationName;
                        descriptionLocation.text = currentFog.location.engDescription;
                    }
                    else if (languageManager.currentLanguage == Language.Indonesian)
                    {
                        nameLocation.text = currentFog.location.indonesianLocationName;
                        descriptionLocation.text = currentFog.location.indonesianDescription;
                    }
                    imageLocation.sprite = currentFog.location.spriteLocation;

                    locationsButton.onClick.RemoveAllListeners();
                    locationsButton.onClick.AddListener(() => AddSound());
                    locationsButton.onClick.AddListener(() => SaveMap());
                    locationsButton.onClick.AddListener(() => LoadScene(currentFog.location.sceneName));
                }
                else if(Vector3.Distance(playerPoint.transform.position, currentCell.transform.position) <= 1)
                {
                    timeToEncounter = 0;
                    currentTimeToEncounter = 0;
                }
            }
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
            speed = 100;
        }
        else
        {
            speed = 50;
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