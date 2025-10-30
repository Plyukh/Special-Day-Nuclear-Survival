using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class SaveScript : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private int[] skillsLvlPoints;
    [SerializeField] private bool[] perks;
    [SerializeField] private Attributes attributes;

    [SerializeField] private GameObject itemsPrefab;
    [SerializeField] private int[] playerItemsID;
    [SerializeField] private float[] playerItemsNumber;
    [SerializeField] private bool[] playerItemsEquipped;

    [SerializeField] private Inventory carInventory;
    [SerializeField] private int[] carItemsID;
    [SerializeField] private float[] carItemsNumber;

    [SerializeField] private SceneManagerScript sceneManager;

    [SerializeField] private Map map;
    [SerializeField] private bool[] fog;

    [SerializeField] private ExperienceSystem experienceSystem;

    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private QuestSystem questSystem;
    [SerializeField] private bool[] questActive;
    [SerializeField] private bool[] questComplete;
    [SerializeField] private bool[] questPartActive;
    [SerializeField] private bool[] questPartComplete;

    [SerializeField] private Settings settings;

    [SerializeField] private TrainingSystem trainingSystem;

    [SerializeField] private Button saveButton;

    [SerializeField] private LanguageManager languageManager;
    private bool startLanguage;

    [SerializeField] private Animator loadingWindowAnimator;
    [SerializeField] private Animator saveWindowAnimator;

    private int activeCoroutines = 0;

    private void Awake()
    {
        attributes = character.Attributes;

        if (GetComponent<SceneManagerScript>().sceneName == "CharecterCreator")
        {
            loadingWindowAnimator.SetBool("Loading", false);

            if (PlayerPrefs.HasKey("StartLanguage"))
            {
                startLanguage = PlayerPrefs.GetInt("StartLanguage") == 1;
                int savedLanguageIndex = PlayerPrefs.HasKey("Language") ? PlayerPrefs.GetInt("Language") : (int)Language.English;
                languageManager.SetLanguage((Language)savedLanguageIndex);
            }
            else
            {
                startLanguage = false;
            }
        }

        if (GetComponent<SceneManagerScript>().sceneName != "CharecterCreator")
        {
            SaveLastSceneName();

            if (PlayerPrefs.HasKey(sceneManager.sceneName + "R"))
            {
                sceneManager.reset = PlayerPrefs.GetInt(sceneManager.sceneName + "R") == 1;
            }

            if(sceneManager.car)
            {
                if (PlayerPrefs.HasKey("QuestsComplete"))
                {
                    string questsCompleteJson = PlayerPrefs.GetString("QuestsComplete");
                    SerializationWrapper<bool> wrapperQuestsComplete = JsonUtility.FromJson<SerializationWrapper<bool>>(questsCompleteJson);
                    bool[] savedQuestsComplete = wrapperQuestsComplete.data;
                    if (savedQuestsComplete[13] == true)
                    {
                        carInventory.transform.parent.gameObject.SetActive(true);
                    }
                }
            }

            if (sceneManager.reset == false)
            {
                Load();
            }
            else
            {
                LoadQuests();
                LoadCharacter();
                LoadStats();
                LoadPerks();
                LoadInventory();
                LoadMap();
                LoadSettings();
            }

            // Check Training
            if (sceneManager.sceneName == "Bunker" && trainingSystem.complete == false)
            {
                trainingSystem.gameObject.SetActive(true);
            }
        }
        else if (PlayerPrefs.HasKey("SceneName"))
        {
            string savedSceneName = PlayerPrefs.GetString("SceneName");
            if (!string.IsNullOrEmpty(savedSceneName))
            {
                sceneManager.LoadScene(savedSceneName);
            }
        }

        if (sceneManager.sceneName == "CharecterCreator")
        {
            if (startLanguage == false)
            {
                startLanguage = true;

                SystemLanguage systemLanguage = Application.systemLanguage;
                if (systemLanguage == SystemLanguage.Belarusian || systemLanguage == SystemLanguage.Russian ||
                    systemLanguage == SystemLanguage.Ukrainian)
                {
                    languageManager.SetLanguage(Language.Russian);
                    SaveLanguage(0);
                }
                else if (systemLanguage == SystemLanguage.Indonesian)
                {
                    languageManager.SetLanguage(Language.Indonesian);
                    SaveLanguage(2);
                }
                else
                {
                    languageManager.SetLanguage(Language.English);
                    SaveLanguage(1);
                }

                PlayerPrefs.SetInt("StartLanguage", 1); // Сохраняем состояние языка
                PlayerPrefs.Save(); // Фиксируем изменения
            }
        }
        else
        {
            LoadLanguage();
        }
    }

    private void Update()
    {
        CheckSave();
    }

    public void CheckSave()
    {
        if(character.stealthSystem != null && character.combatSystem != null)
        {
            if (character.stealthSystem.stealth == false && character.combatSystem.combat == false)
            {
                saveButton.interactable = true;
                saveButton.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                saveButton.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                saveButton.interactable = false;
                saveButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                saveButton.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    public void SaveCharacter()
    {
        // Сохранение имени персонажа
        PlayerPrefs.SetString("CharacterName", character.characterName);

        // Сохранение пола персонажа
        PlayerPrefs.SetInt("CharacterMale", character.male ? 1 : 0);

        // Сохранение индексных параметров
        PlayerPrefs.SetInt("HairIndex", character.currentHairIndex);
        PlayerPrefs.SetInt("BeardIndex", character.currentBeardIndex);
        PlayerPrefs.SetInt("AccessoriesIndex", character.currentAccessoriesIndex);
        PlayerPrefs.SetInt("SkinIndex", character.currentSkinIndex);

        // Сохранение атрибутов
        PlayerPrefs.SetInt("Attributes_Strength", character.Attributes.Strength);
        PlayerPrefs.SetInt("Attributes_Agility", character.Attributes.Agility);
        PlayerPrefs.SetInt("Attributes_Intelligence", character.Attributes.Intelligence);
        PlayerPrefs.SetInt("Attributes_Charisma", character.Attributes.Charisma);
        PlayerPrefs.SetInt("Attributes_Points", character.Attributes.points);

        PlayerPrefs.Save(); // Фиксация изменений
    }

    public void LoadCharacter()
    {
        activeCoroutines++;
        StartCoroutine(LoadCharacterCoroutine());
    }

    private IEnumerator LoadCharacterCoroutine()
    {
        // Загрузка имени персонажа
        if (PlayerPrefs.HasKey("CharacterName"))
        {
            character.characterName = PlayerPrefs.GetString("CharacterName");
        }

        // Загрузка пола персонажа
        if (PlayerPrefs.HasKey("CharacterMale"))
        {
            character.male = PlayerPrefs.GetInt("CharacterMale") == 1;
        }

        // Загрузка индексных параметров
        if (PlayerPrefs.HasKey("HairIndex"))
        {
            character.currentHairIndex = PlayerPrefs.GetInt("HairIndex");
        }
        if (PlayerPrefs.HasKey("BeardIndex"))
        {
            character.currentBeardIndex = PlayerPrefs.GetInt("BeardIndex");
        }
        if (PlayerPrefs.HasKey("AccessoriesIndex"))
        {
            character.currentAccessoriesIndex = PlayerPrefs.GetInt("AccessoriesIndex");
        }
        if (PlayerPrefs.HasKey("SkinIndex"))
        {
            character.currentSkinIndex = PlayerPrefs.GetInt("SkinIndex");
        }

        // Загрузка атрибутов
        if (PlayerPrefs.HasKey("Attributes_Strength"))
        {
            character.Attributes.Strength = PlayerPrefs.GetInt("Attributes_Strength");
            character.Attributes.Agility = PlayerPrefs.GetInt("Attributes_Agility");
            character.Attributes.Intelligence = PlayerPrefs.GetInt("Attributes_Intelligence");
            character.Attributes.Charisma = PlayerPrefs.GetInt("Attributes_Charisma");
            character.Attributes.points = PlayerPrefs.GetInt("Attributes_Points");
        }

        yield return null; // Асинхронная пауза

        activeCoroutines--;

        if (activeCoroutines == 0 && loadingWindowAnimator != null)
        {
            loadingWindowAnimator.SetBool("Loading", false);
        }
    }

    public void SaveStats()
    {
        for (int i = 0; i < skillsLvlPoints.Length; i++)
        {
            skillsLvlPoints[i] = character.CharacterSkills[i].lvlPoints;
        }

        string skillsLvlPointsJson = JsonUtility.ToJson(new SerializationWrapper<int>(skillsLvlPoints));

        PlayerPrefs.SetString("SkillsLvlPoints", skillsLvlPointsJson);
        PlayerPrefs.SetFloat("CurrentHP", character.GetComponent<HealthSystem>().health);
        PlayerPrefs.SetFloat("XP", experienceSystem.GetCurrentXP);
        PlayerPrefs.SetInt("LVL", experienceSystem.GetCurrentLVL);
        PlayerPrefs.SetFloat("CurrentRadiation", character.GetComponent<Radiation>().currentRad);

        PlayerPrefs.Save(); // Сохранение изменений
    }

    public void LoadStats()
    {
        // Увеличиваем счётчик активных корутин
        activeCoroutines++;
        StartCoroutine(LoadStatsCoroutine());
    }

    private IEnumerator LoadStatsCoroutine()
    {
        if (PlayerPrefs.HasKey("SkillsLvlPoints"))
        {
            string SkillsLvlPointsJson = PlayerPrefs.GetString("SkillsLvlPoints");
            SerializationWrapper<int> wrapperSkillsLvlPoints = JsonUtility.FromJson<SerializationWrapper<int>>(SkillsLvlPointsJson);
            int[] savedSkillsLvlPoints = wrapperSkillsLvlPoints.data;

            for (int i = 0; i < character.CharacterSkills.Length; i++)
            {
                character.CharacterSkills[i].lvlPoints = savedSkillsLvlPoints[i];
            }
        }

        // Загрузка текущего здоровья персонажа
        if (PlayerPrefs.HasKey("CurrentHP"))
        {
            character.GetComponent<HealthSystem>().health = PlayerPrefs.GetFloat("CurrentHP");
        }

        // Загрузка опыта и уровня
        if (PlayerPrefs.HasKey("XP") && PlayerPrefs.HasKey("LVL"))
        {
            float xp = PlayerPrefs.GetFloat("XP");
            int lvl = PlayerPrefs.GetInt("LVL");
            experienceSystem.SetXP_LVL(xp, lvl);
        }

        // Загрузка текущей радиации
        if (PlayerPrefs.HasKey("CurrentRadiation"))
        {
            character.GetComponent<Radiation>().currentRad = PlayerPrefs.GetFloat("CurrentRadiation");
        }

        // Запуск системы радиации после загрузки
        character.GetComponent<Radiation>().StartRad();

        yield return null; // Асинхронная пауза

        // Уменьшаем счётчик активных корутин
        activeCoroutines--;

        // Проверяем, завершены ли все корутины
        if (activeCoroutines == 0 && loadingWindowAnimator != null)
        {
            loadingWindowAnimator.SetBool("Loading", false); // Отключаем анимацию загрузки
        }
    }

    public void SavePerks()
    {
        PlayerPrefs.SetInt("PerksPoints", character.PerkSystem.points);

        // Сериализация массива Perk[]

        for (int i = 0; i < perks.Length; i++)
        {
            perks[i] = character.PerkSystem.perks[i].Active;
        }

        // Сериализация массива булевых значений
        string perksJson = JsonUtility.ToJson(new SerializationWrapper<bool>(perks));
        PlayerPrefs.SetString("Perks", perksJson);

        PlayerPrefs.Save();
    }


    public void LoadPerks()
    {
        activeCoroutines++;
        StartCoroutine(LoadPerksCoroutine());
    }

    private IEnumerator LoadPerksCoroutine()
    {
        if (PlayerPrefs.HasKey("PerksPoints"))
        {
            character.PerkSystem.points = PlayerPrefs.GetInt("PerksPoints");
        }

        if (PlayerPrefs.HasKey("Perks"))
        {
            string perksJson = PlayerPrefs.GetString("Perks");
            SerializationWrapper<bool> wrapper = JsonUtility.FromJson<SerializationWrapper<bool>>(perksJson);

            // Загрузка сохранённых Perk[]
            bool[] savedPerks = wrapper.data;

            for (int i = 0; i < savedPerks.Length; i++)
            {
                if (i < character.PerkSystem.perks.Length) // Защита от выхода за границы массива
                {
                    character.PerkSystem.perks[i].Active = savedPerks[i];
                }
            }
        }

        yield return null;

        activeCoroutines--;

        if (activeCoroutines == 0 && loadingWindowAnimator != null)
        {
            loadingWindowAnimator.SetBool("Loading", false);
        }
    }

    public void SaveInventory()
    {
        PlayerPrefs.SetInt("Money", character.Inventory.money);

        for (int i = 0; i < character.Inventory.items.Length; i++)
        {
            if(character.Inventory.items[i] != null)
            {
                playerItemsID[i] = character.Inventory.items[i].ID;
                playerItemsNumber[i] = character.Inventory.items[i].number;
                playerItemsEquipped[i] = character.Inventory.items[i].equipped;
            }
            else
            {
                playerItemsID[i] = -1;
            }
        }

        for (int i = 0; i < carInventory.items.Length; i++)
        {
            if (carInventory.items[i] != null)
            {
                carItemsID[i] = carInventory.items[i].ID;
                carItemsNumber[i] = carInventory.items[i].number;
            }
            else
            {
                carItemsID[i] = -1;
            }
        }

        string carItemsIDJson = JsonUtility.ToJson(new SerializationWrapper<int>(carItemsID));
        string carItemsNumberJson = JsonUtility.ToJson(new SerializationWrapper<float>(carItemsNumber));

        PlayerPrefs.SetString("CarItemsID", carItemsIDJson);
        PlayerPrefs.SetString("CarItemsNumber", carItemsNumberJson);

        // Сериализация массива предметов в JSON
        string itemsIDJson = JsonUtility.ToJson(new SerializationWrapper<int>(playerItemsID));
        string itemsNumberJson = JsonUtility.ToJson(new SerializationWrapper<float>(playerItemsNumber));
        string itemsEquippedJson = JsonUtility.ToJson(new SerializationWrapper<bool>(playerItemsEquipped));

        PlayerPrefs.SetString("PlayerItemsID", itemsIDJson);
        PlayerPrefs.SetString("PlayerItemsNumber", itemsNumberJson);
        PlayerPrefs.SetString("PlayerItemsEquipped", itemsEquippedJson);

        PlayerPrefs.Save(); // Сохранение изменений
    }

    public void LoadInventory()
    {
        // Увеличиваем счётчик активных корутин
        activeCoroutines++;
        StartCoroutine(LoadInventoryCoroutine());
    }
    private IEnumerator LoadInventoryCoroutine()
    {
        // Загрузка денег
        if (PlayerPrefs.HasKey("Money"))
        {
            character.Inventory.money = PlayerPrefs.GetInt("Money");
        }

        // Загрузка предметов в инвентарь
        if (PlayerPrefs.HasKey("PlayerItemsID"))
        {
            string itemsIDJson = PlayerPrefs.GetString("PlayerItemsID");
            string itemsNumberJson = PlayerPrefs.GetString("PlayerItemsNumber");
            string itemsEquippedJson = PlayerPrefs.GetString("PlayerItemsEquipped");

            SerializationWrapper<int> wrapperID = JsonUtility.FromJson<SerializationWrapper<int>>(itemsIDJson);
            SerializationWrapper<float> wrapperNumber = JsonUtility.FromJson<SerializationWrapper<float>>(itemsNumberJson);
            SerializationWrapper<bool> wrapperEquipped = JsonUtility.FromJson<SerializationWrapper<bool>>(itemsEquippedJson);

            int[] savedItemsID = wrapperID.data;
            float[] savedItemsNumber = wrapperNumber.data;
            bool[] savedItemsEquipped = wrapperEquipped.data;

            for (int i = 0; i < character.Inventory.items.Length; i++)
            {
                if (savedItemsID[i] >= 0)
                {
                    Item newItem = Instantiate(itemsPrefab.transform.GetChild(savedItemsID[i]).GetComponent<Item>(), character.Inventory.transform);

                    newItem.ID = savedItemsID[i];
                    newItem.number = savedItemsNumber[i];
                    newItem.equipped = savedItemsEquipped[i];

                    character.Inventory.items[i] = newItem;
                }
            }
        }
        if (PlayerPrefs.HasKey("CarItemsID"))
        {
            string carItemsIDJson = PlayerPrefs.GetString("CarItemsID");
            string carItemsNumberJson = PlayerPrefs.GetString("CarItemsNumber");

            SerializationWrapper<int> wrapperCarID = JsonUtility.FromJson<SerializationWrapper<int>>(carItemsIDJson);
            SerializationWrapper<float> wrapperCarNumber = JsonUtility.FromJson<SerializationWrapper<float>>(carItemsNumberJson);

            int[] savedCarItemsID = wrapperCarID.data;
            float[] savedCarItemsNumber = wrapperCarNumber.data;
            for (int i = 0; i < carInventory.items.Length; i++)
            {
                if (savedCarItemsID[i] >= 0)
                {
                    Item newItem = Instantiate(itemsPrefab.transform.GetChild(savedCarItemsID[i]).GetComponent<Item>(), carInventory.transform);

                    newItem.ID = savedCarItemsID[i];
                    newItem.number = savedCarItemsNumber[i];

                    carInventory.items[i] = newItem;
                }
            }
        }

        yield return null; // Асинхронная пауза

        // Уменьшаем счётчик активных корутин
        activeCoroutines--;

        // Проверяем, завершены ли все корутины
        if (activeCoroutines == 0 && loadingWindowAnimator != null)
        {
            loadingWindowAnimator.SetBool("Loading", false); // Отключаем анимацию загрузки
        }
    }

    public void SaveQuests()
    {
        for (int i = 0; i < questSystem.quests.Length; i++)
        {
            questComplete[i] = questSystem.quests[i].Complete;
            questActive[i] = questSystem.quests[i].Active;
            for (int j = 0; j < questSystem.quests[i].QuestParts.Length; j++)
            {
                questPartActive[j + i * 4] = questSystem.quests[i].QuestParts[j].Active;
                questPartComplete[j + i * 4] = questSystem.quests[i].QuestParts[j].Complete;
            }
        }

        // Сериализация массива квестов в JSON
        string questActiveJson = JsonUtility.ToJson(new SerializationWrapper<bool>(questActive));
        string questCompleteJson = JsonUtility.ToJson(new SerializationWrapper<bool>(questComplete));
        string questPartActiveJson = JsonUtility.ToJson(new SerializationWrapper<bool>(questPartActive));
        string questPartCompleteJson = JsonUtility.ToJson(new SerializationWrapper<bool>(questPartComplete));
        PlayerPrefs.SetString("QuestsActive", questActiveJson);
        PlayerPrefs.SetString("QuestsComplete", questCompleteJson);
        PlayerPrefs.SetString("QuestsPartActive", questPartActiveJson);
        PlayerPrefs.SetString("QuestsPartComplete", questPartCompleteJson);

        PlayerPrefs.Save(); // Сохранение изменений
    }

    public void LoadQuests()
    {
        // Увеличиваем счётчик активных корутин
        activeCoroutines++;
        StartCoroutine(LoadQuestsCoroutine());
    }

    private IEnumerator LoadQuestsCoroutine()
    {
        if (PlayerPrefs.HasKey("QuestsActive"))
        {
            // Десериализация JSON обратно в массив квестов
            string questsActiveJson = PlayerPrefs.GetString("QuestsActive");
            string questsCompleteJson = PlayerPrefs.GetString("QuestsComplete");
            string questsPartActiveJson = PlayerPrefs.GetString("QuestsPartActive");
            string questsPartCompleteJson = PlayerPrefs.GetString("QuestsPartComplete");
            
            SerializationWrapper<bool> wrapperQuestsActive = JsonUtility.FromJson<SerializationWrapper<bool>>(questsActiveJson);
            SerializationWrapper<bool> wrapperQuestsComplete = JsonUtility.FromJson<SerializationWrapper<bool>>(questsCompleteJson);
            SerializationWrapper<bool> wrapperQuestsPartActive = JsonUtility.FromJson<SerializationWrapper<bool>>(questsPartActiveJson);
            SerializationWrapper<bool> wrapperQuestsPartComplete = JsonUtility.FromJson<SerializationWrapper<bool>>(questsPartCompleteJson);
            
            bool[] savedQuestsActive = wrapperQuestsActive.data;
            bool[] savedQuestsComplete = wrapperQuestsComplete.data;
            bool[] savedQuestsPartActive = wrapperQuestsPartActive.data;
            bool[] savedQuestsPartComplete = wrapperQuestsPartComplete.data;

            for (int i = 0; i < questSystem.quests.Length; i++)
            {
                // Загрузка основной информации о квесте
                questSystem.quests[i].Active = savedQuestsActive[i];
                questSystem.quests[i].Complete = savedQuestsComplete[i];

                // Загрузка деталей для каждого квеста
                for (int j = 0; j < questSystem.quests[i].QuestParts.Length; j++)
                {
                    questSystem.quests[i].QuestParts[j].Active = savedQuestsPartActive[j + i * 4];
                    questSystem.quests[i].QuestParts[j].Complete = savedQuestsPartComplete[j + i * 4];
                }
            }

            yield return null; // Асинхронная пауза после обработки данных
        }

        // Уменьшаем счётчик активных корутин
        activeCoroutines--;

        // Проверяем, завершены ли все корутины
        if (activeCoroutines == 0 && loadingWindowAnimator != null)
        {
            loadingWindowAnimator.SetBool("Loading", false); // Отключаем анимацию загрузки
        }
    }

    public void SaveMap()
    {
        // Сохранение состояния "тумана войны"

        for (int i = 0; i < fog.Length; i++)
        {
            fog[i] = map.Fog[i].find;
        }

        string fogJson = JsonUtility.ToJson(new SerializationWrapper<bool>(fog));
        PlayerPrefs.SetString("Fog", fogJson);

        // Сохранение состояния случайных встреч
        string encountersJson = JsonUtility.ToJson(new SerializationWrapper<bool>(map.attendedRandomEncounter));
        PlayerPrefs.SetString("RandomEncounters", encountersJson);

        // Сохранение позиции игрока на карте
        if (map.lastCell != null)
        {
            for (int i = 0; i < map.Fog.Length; i++)
            {
                if (map.Fog[i].name == map.lastCell.name)
                {
                    PlayerPrefs.SetInt("PlayerMapPotionIndex", i);
                    break;
                }
            }
        }

        PlayerPrefs.Save(); // Сохранение изменений
    }
    public void LoadMap()
    {
        // Увеличиваем счётчик активных корутин
        activeCoroutines++;
        StartCoroutine(LoadMapCoroutine());
    }

    private IEnumerator LoadMapCoroutine()
    {
        // Загрузка состояния "тумана войны"
        if (PlayerPrefs.HasKey("Fog"))
        {
            string fogJson = PlayerPrefs.GetString("Fog");
            SerializationWrapper<bool> fogWrapper = JsonUtility.FromJson<SerializationWrapper<bool>>(fogJson);
            bool[] savedFog = fogWrapper.data;

            for (int i = 0; i < fog.Length; i++)
            {
                map.Fog[i].find = savedFog[i];
            }
            yield return null; // Асинхронная пауза
        }

        // Загрузка состояния случайных встреч
        if (PlayerPrefs.HasKey("RandomEncounters"))
        {
            string encountersJson = PlayerPrefs.GetString("RandomEncounters");
            SerializationWrapper<bool> encountersWrapper = JsonUtility.FromJson<SerializationWrapper<bool>>(encountersJson);
            bool[] savedEncounters = encountersWrapper.data;

            for (int i = 0; i < map.attendedRandomEncounter.Length; i++)
            {
                if (i < savedEncounters.Length)
                {
                    map.attendedRandomEncounter[i] = savedEncounters[i];
                }
            }
            yield return null; // Асинхронная пауза
        }

        // Загрузка позиции игрока на карте
        if (PlayerPrefs.HasKey("PlayerMapPotionIndex"))
        {
            int playerMapPointIndex = PlayerPrefs.GetInt("PlayerMapPotionIndex");
            if (playerMapPointIndex < map.Fog.Length)
            {
                map.PlayerMapPoint.transform.position = map.Fog[playerMapPointIndex].transform.position;
            }
            yield return null; // Асинхронная пауза
        }

        // Уменьшаем счётчик активных корутин
        activeCoroutines--;

        // Проверяем, завершены ли все корутины
        if (activeCoroutines == 0 && loadingWindowAnimator != null)
        {
            loadingWindowAnimator.SetBool("Loading", false); // Отключаем анимацию загрузки
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("Settings_QualityLevel", settings.qualityLevel);
        PlayerPrefs.SetInt("Settings_Shadows", settings.shadows ? 1 : 0);
        PlayerPrefs.SetInt("Settings_FPSTarget", settings.fpsTarget);
        PlayerPrefs.SetInt("Settings_FPS", settings.fps ? 1 : 0);
        PlayerPrefs.SetFloat("Settings_MusicValue", settings.musicValue);
        PlayerPrefs.SetFloat("Settings_SoundsValue", settings.soundsValue);
        PlayerPrefs.SetInt("Language", settings.languageIndex); // Сохранение языка как индекс

        PlayerPrefs.Save(); // Сохранение изменений
    }

    public void LoadSettings()
    {
        // Увеличиваем счётчик активных корутин
        activeCoroutines++;
        StartCoroutine(LoadSettingsCoroutine());
    }

    private IEnumerator LoadSettingsCoroutine()
    {
        if (PlayerPrefs.HasKey("Settings_QualityLevel"))
        {
            settings.qualityLevel = PlayerPrefs.GetInt("Settings_QualityLevel");
            yield return null; // Асинхронная пауза
        }

        if (PlayerPrefs.HasKey("Settings_Shadows"))
        {
            settings.shadows = PlayerPrefs.GetInt("Settings_Shadows") == 1;
            yield return null;
        }

        if (PlayerPrefs.HasKey("Settings_FPSTarget"))
        {
            settings.fpsTarget = PlayerPrefs.GetInt("Settings_FPSTarget");
            yield return null;
        }

        if (PlayerPrefs.HasKey("Settings_FPS"))
        {
            settings.fps = PlayerPrefs.GetInt("Settings_FPS") == 1;
            yield return null;
        }

        if (PlayerPrefs.HasKey("Settings_MusicValue"))
        {
            settings.musicValue = PlayerPrefs.GetFloat("Settings_MusicValue");
            yield return null;
        }

        if (PlayerPrefs.HasKey("Settings_SoundsValue"))
        {
            settings.soundsValue = PlayerPrefs.GetFloat("Settings_SoundsValue");
            yield return null;
        }

        if (PlayerPrefs.HasKey("Language"))
        {
            settings.languageIndex = PlayerPrefs.GetInt("Language");
            yield return null;
        }

        settings.UpdateSettings(); // Применение настроек после загрузки

        // Уменьшаем счётчик активных корутин
        activeCoroutines--;

        // Проверяем, завершены ли все корутины
        if (activeCoroutines == 0 && loadingWindowAnimator != null)
        {
            loadingWindowAnimator.SetBool("Loading", false); // Отключаем анимацию загрузки
        }
    }

    public void SaveLastSceneName()
    {
        PlayerPrefs.SetString("SceneName", sceneManager.sceneName);
        PlayerPrefs.Save(); // Сохранение изменений
    }

    public void SaveTraining()
    {
        if (trainingSystem != null)
        {
            PlayerPrefs.SetInt("Training", trainingSystem.complete ? 1 : 0); // Сохранение как целое число (1 или 0)
            PlayerPrefs.Save(); // Фиксация изменений
        }
    }

    public void LoadTraining()
    {
        // Увеличиваем счётчик активных корутин
        activeCoroutines++;
        StartCoroutine(LoadTrainingCoroutine());
    }

    private IEnumerator LoadTrainingCoroutine()
    {
        if (trainingSystem != null)
        {
            if (PlayerPrefs.HasKey("Training"))
            {
                // Асинхронная загрузка состояния тренировочной системы
                trainingSystem.complete = PlayerPrefs.GetInt("Training") == 1;
                yield return null; // Асинхронная пауза
            }
        }

        // Уменьшаем счётчик активных корутин
        activeCoroutines--;

        // Проверяем, завершены ли все корутины
        if (activeCoroutines == 0 && loadingWindowAnimator != null)
        {
            loadingWindowAnimator.SetBool("Loading", false); // Отключаем анимацию загрузки
        }
    }

    public void SaveScene()
    {
        for (int i = 0; i < sceneManager.rooms.Length; i++)
        {
            sceneManager.find[i] = sceneManager.rooms[i].find;
        }

        for (int i = 0; i < sceneManager.dialogues.Length; i++)
        {
            sceneManager.currentNodes[i] = sceneManager.dialogues[i].currentNode;
            sceneManager.health[i] = sceneManager.characters[i].GetComponent<HealthSystem>().health;
            sceneManager.aggressive[i] = sceneManager.characters[i].combatSystem.Aggressive;
            sceneManager.canDialogue[i] = sceneManager.characters[i].combatSystem.CanDialogue;
            sceneManager.hasLeft[i] = sceneManager.characters[i].hasLeft;
        }

        for (int i = 0; i < sceneManager.inventories.Length; i++)
        {
            sceneManager.money[i] = sceneManager.inventories[i].money;

            for (int j = 0; j < sceneManager.inventories[i].items.Length; j++)
            {
                if (sceneManager.inventories[i].items[j] != null)
                {
                    sceneManager.itemsID[j + i * 25] = sceneManager.inventories[i].items[j].ID;
                    sceneManager.itemsNumber[j + i * 25] = sceneManager.inventories[i].items[j].number;
                    sceneManager.itemsEquipped[j + i * 25] = sceneManager.inventories[i].items[j].equipped;
                    sceneManager.itemsSell[j + i * 25] = sceneManager.inventories[i].items[j].sell;
                }
                else
                {
                    sceneManager.itemsID[j + i * 25] = -1;
                }
            }
        }

        for (int i = 0; i < sceneManager.powerBoxes.Length; i++)
        {
            sceneManager.works[i] = sceneManager.powerBoxes[i].works;
        }

        for (int i = 0; i < sceneManager.interactableObjects.Length; i++)
        {
            if (sceneManager.interactableObjects[i].GetComponent<Door>())
            {
                sceneManager.openDoors[i] = sceneManager.interactableObjects[i].GetComponent<Door>().open;
            }

            if (sceneManager.interactableObjects[i].needSkill != null)
            {
                sceneManager.skills[i] = sceneManager.interactableObjects[i].needSkill.attributePoints;
            }
            else
            {
                sceneManager.skills[i] = 0;
            }
        }

        // Сохранение позиции игрока
        PlayerPrefs.SetFloat(sceneManager.sceneName + "_PlayerPositionX", character.transform.position.x);
        PlayerPrefs.SetFloat(sceneManager.sceneName + "_PlayerPositionY", character.transform.position.y);
        PlayerPrefs.SetFloat(sceneManager.sceneName + "_PlayerPositionZ", character.transform.position.z);

        // Сохранение текущей комнаты
        PlayerPrefs.SetString(sceneManager.sceneName + "_CurrentRoomName", character.characterMovement.CurrentRoom.name);

        // Сохранение массивов данных find
        PlayerPrefs.SetString(sceneManager.sceneName + "_Find", JsonUtility.ToJson(new SerializationWrapper<bool>(sceneManager.find)));
        
        // Сохранение массивов данных currentNodes
        PlayerPrefs.SetString(sceneManager.sceneName + "_CurrentNodes", JsonUtility.ToJson(new SerializationWrapper<int>(sceneManager.currentNodes)));

        // Сохранение здоровья персонажей
        PlayerPrefs.SetString(sceneManager.sceneName + "_Health", JsonUtility.ToJson(new SerializationWrapper<float>(sceneManager.health)));

        // Сохранение остальных данных о персонажах
        PlayerPrefs.SetString(sceneManager.sceneName + "_DeathHair", JsonUtility.ToJson(new SerializationWrapper<bool>(sceneManager.deathHair)));
        PlayerPrefs.SetString(sceneManager.sceneName + "_Aggressive", JsonUtility.ToJson(new SerializationWrapper<bool>(sceneManager.aggressive)));
        PlayerPrefs.SetString(sceneManager.sceneName + "_CanDialogue", JsonUtility.ToJson(new SerializationWrapper<bool>(sceneManager.canDialogue)));
        PlayerPrefs.SetString(sceneManager.sceneName + "_HasLeft", JsonUtility.ToJson(new SerializationWrapper<bool>(sceneManager.hasLeft)));

        // Сохранение состояния объектов (powerBoxes, repairObjects и т.д.)
        PlayerPrefs.SetString(sceneManager.sceneName + "_Works", JsonUtility.ToJson(new SerializationWrapper<bool>(sceneManager.works)));
        PlayerPrefs.SetString(sceneManager.sceneName + "_OpenDoors", JsonUtility.ToJson(new SerializationWrapper<bool>(sceneManager.openDoors)));
        PlayerPrefs.SetString(sceneManager.sceneName + "_LastRepairObject", JsonUtility.ToJson(new SerializationWrapper<string>(sceneManager.lastRepairObject)));
        PlayerPrefs.SetString(sceneManager.sceneName + "_Skills", JsonUtility.ToJson(new SerializationWrapper<int>(sceneManager.skills)));

        // Сохранение инвентаря
        PlayerPrefs.SetString(sceneManager.sceneName + "_Money", JsonUtility.ToJson(new SerializationWrapper<int>(sceneManager.money)));
        PlayerPrefs.SetString(sceneManager.sceneName + "_ItemsID", JsonUtility.ToJson(new SerializationWrapper<int>(sceneManager.itemsID)));
        PlayerPrefs.SetString(sceneManager.sceneName + "_ItemsNumber", JsonUtility.ToJson(new SerializationWrapper<float>(sceneManager.itemsNumber)));
        PlayerPrefs.SetString(sceneManager.sceneName + "_ItemsEquipped", JsonUtility.ToJson(new SerializationWrapper<bool>(sceneManager.itemsEquipped)));
        PlayerPrefs.SetString(sceneManager.sceneName + "_ItemsSell", JsonUtility.ToJson(new SerializationWrapper<bool>(sceneManager.itemsSell)));

        PlayerPrefs.Save(); // Сохранение изменений
    }
    public void LoadScene()
    {
        if (PlayerPrefs.HasKey(sceneManager.sceneName + "_PlayerPositionX"))
        {
            character.GetComponent<NavMeshAgent>().enabled = false;
            character.transform.position = new Vector3(PlayerPrefs.GetFloat(sceneManager.sceneName + "_PlayerPositionX"), PlayerPrefs.GetFloat(sceneManager.sceneName + "_PlayerPositionY"), PlayerPrefs.GetFloat(sceneManager.sceneName + "_PlayerPositionZ"));
            character.GetComponent<NavMeshAgent>().enabled = true;
        }
        if (PlayerPrefs.HasKey(sceneManager.sceneName + "_CurrentRoomName"))
        {
            // Загрузка текущей комнаты
            string currentRoomName = PlayerPrefs.GetString(sceneManager.sceneName + "_CurrentRoomName");
            foreach (var room in sceneManager.rooms)
            {
                if (room.name == currentRoomName)
                {
                    character.characterMovement.CurrentRoom = room.gameObject;
                    break;
                }
            }
        }
        if(PlayerPrefs.HasKey(sceneManager.sceneName + "_Find"))
        {
            string sceneManagerFindJson = PlayerPrefs.GetString(sceneManager.sceneName + "_Find");
            SerializationWrapper<bool> wrappersceneManagerFind = JsonUtility.FromJson<SerializationWrapper<bool>>(sceneManagerFindJson);
            bool[] savedSceneManagerFind = wrappersceneManagerFind.data;

            for (int i = 0; i < sceneManager.rooms.Length; i++)
            {
                sceneManager.rooms[i].find = savedSceneManagerFind[i];
                if (sceneManager.rooms[i].find)
                {
                    if(sceneManager.rooms[i].Mesh())
                    {
                        sceneManager.rooms[i].CubeObject.Outline = sceneManager.rooms[i].CubeObject.GetComponent<Outline>();
                        sceneManager.rooms[i].MeshDisabled();
                    }
                }
            }
        }
        if (PlayerPrefs.HasKey(sceneManager.sceneName + "_CurrentNodes"))
        {
            string sceneManagerCurrentNodesJson = PlayerPrefs.GetString(sceneManager.sceneName + "_CurrentNodes");
            SerializationWrapper<int> wrappersceneManagerCurrentNodes = JsonUtility.FromJson<SerializationWrapper<int>>(sceneManagerCurrentNodesJson);
            int[] savedSceneManagerCurrentNodes = wrappersceneManagerCurrentNodes.data;

            for (int i = 0; i < sceneManager.characters.Length; i++)
            {
                sceneManager.dialogues[i].currentNode = savedSceneManagerCurrentNodes[i];
            }
        }
        if (PlayerPrefs.HasKey(sceneManager.sceneName + "_Health"))
        {
            string sceneManagerHealthJson = PlayerPrefs.GetString(sceneManager.sceneName + "_Health");
            SerializationWrapper<float> wrappersceneManagerHealth = JsonUtility.FromJson<SerializationWrapper<float>>(sceneManagerHealthJson);
            float[] savedSceneManagerHealth = wrappersceneManagerHealth.data;

            for (int i = 0; i < sceneManager.characters.Length; i++)
            {
                sceneManager.characters[i].GetComponent<HealthSystem>().health = savedSceneManagerHealth[i];
            }
        }
        if (PlayerPrefs.HasKey(sceneManager.sceneName + "_DeathHair"))
        {
            string sceneManagerDeathHairJson = PlayerPrefs.GetString(sceneManager.sceneName + "_DeathHair");
            SerializationWrapper<bool> wrappersceneManagerDeathHair = JsonUtility.FromJson<SerializationWrapper<bool>>(sceneManagerDeathHairJson);
            bool[] savedSceneManagerDeathHair = wrappersceneManagerDeathHair.data;

            for (int i = 0; i < sceneManager.deathHair.Length; i++)
            {
                sceneManager.deathHair[i] = savedSceneManagerDeathHair[i];
            }
        }
        if (PlayerPrefs.HasKey(sceneManager.sceneName + "_Aggressive"))
        {
            string sceneManagerAggressiveJson = PlayerPrefs.GetString(sceneManager.sceneName + "_Aggressive");
            SerializationWrapper<bool> wrappersceneManagerAggressive = JsonUtility.FromJson<SerializationWrapper<bool>>(sceneManagerAggressiveJson);
            bool[] savedSceneManagerAggressive = wrappersceneManagerAggressive.data;

            for (int i = 0; i < sceneManager.characters.Length; i++)
            {
                sceneManager.characters[i].combatSystem.Aggressive = savedSceneManagerAggressive[i];
            }
        }
        if (PlayerPrefs.HasKey(sceneManager.sceneName + "_CanDialogue"))
        {
            string sceneManagerCanDialogueJson = PlayerPrefs.GetString(sceneManager.sceneName + "_CanDialogue");
            SerializationWrapper<bool> wrappersceneManagerCanDialogue = JsonUtility.FromJson<SerializationWrapper<bool>>(sceneManagerCanDialogueJson);
            bool[] savedSceneManagerCanDialogue = wrappersceneManagerCanDialogue.data;

            for (int i = 0; i < sceneManager.characters.Length; i++)
            {
                sceneManager.characters[i].combatSystem.CanDialogue = savedSceneManagerCanDialogue[i];
            }
        }
        if (PlayerPrefs.HasKey(sceneManager.sceneName + "_HasLeft"))
        {
            string sceneManagerHasLeftJson = PlayerPrefs.GetString(sceneManager.sceneName + "_HasLeft");
            SerializationWrapper<bool> wrappersceneManagerHasLeft = JsonUtility.FromJson<SerializationWrapper<bool>>(sceneManagerHasLeftJson);
            bool[] savedSceneManagerHasLeft = wrappersceneManagerHasLeft.data;

            for (int i = 0; i < sceneManager.characters.Length; i++)
            {
                sceneManager.characters[i].hasLeft = savedSceneManagerHasLeft[i];
            }
        }
        if (PlayerPrefs.HasKey(sceneManager.sceneName + "_Works"))
        {
            string sceneManagerWorksJson = PlayerPrefs.GetString(sceneManager.sceneName + "_Works");
            SerializationWrapper<bool> wrappersceneManagerWorks = JsonUtility.FromJson<SerializationWrapper<bool>>(sceneManagerWorksJson);
            bool[] savedSceneManagerWorks = wrappersceneManagerWorks.data;

            for (int i = 0; i < sceneManager.powerBoxes.Length; i++)
            {
                sceneManager.powerBoxes[i].works = savedSceneManagerWorks[i];
                sceneManager.powerBoxes[i].works = !sceneManager.powerBoxes[i].works;
            }
        }
        if (PlayerPrefs.HasKey(sceneManager.sceneName + "_LastRepairObject"))
        {
            string sceneManagerLastRepairObjectJson = PlayerPrefs.GetString(sceneManager.sceneName + "_LastRepairObject");
            SerializationWrapper<string> wrappersceneManagerLastRepairObject = JsonUtility.FromJson<SerializationWrapper<string>>(sceneManagerLastRepairObjectJson);
            string[] savedSceneManagerLastRepairObject = wrappersceneManagerLastRepairObject.data;

            for (int i = 0; i < sceneManager.lastRepairObject.Length; i++)
            {
                sceneManager.lastRepairObject[i] = savedSceneManagerLastRepairObject[i];
            }
        }
        if (PlayerPrefs.HasKey(sceneManager.sceneName + "_Skills"))
        {
            string sceneManagerSkillsJson = PlayerPrefs.GetString(sceneManager.sceneName + "_Skills");
            SerializationWrapper<int> wrappersceneManagerSkills = JsonUtility.FromJson<SerializationWrapper<int>>(sceneManagerSkillsJson);
            int[] savedSceneManagerSkills = wrappersceneManagerSkills.data;

            for (int i = 0; i < sceneManager.interactableObjects.Length; i++)
            {
                if (sceneManager.interactableObjects[i].needSkill != null)
                {
                    if (savedSceneManagerSkills[i] > 0)
                    {
                        sceneManager.interactableObjects[i].needSkill.attributePoints = savedSceneManagerSkills[i];
                    }
                    else
                    {
                        Destroy(sceneManager.interactableObjects[i].GetComponent<Skill>());
                    }
                }
            }

            if(PlayerPrefs.HasKey(sceneManager.sceneName + "_OpenDoors"))
            {
                string sceneManagerOpenDoorsJson = PlayerPrefs.GetString(sceneManager.sceneName + "_OpenDoors");
                SerializationWrapper<bool> wrappersceneManagerOpenDoors = JsonUtility.FromJson<SerializationWrapper<bool>>(sceneManagerOpenDoorsJson);
                bool[] savedSceneManagerOpenDoors = wrappersceneManagerOpenDoors.data;

                for (int i = 0; i < sceneManager.openDoors.Length; i++)
                {
                    sceneManager.openDoors[i] = savedSceneManagerOpenDoors[i];
                }
            }
        }
        if (PlayerPrefs.HasKey(sceneManager.sceneName + "_Money"))
        {
            string sceneManagerMoneyJson = PlayerPrefs.GetString(sceneManager.sceneName + "_Money");
            SerializationWrapper<int> wrappersceneManagerMoney = JsonUtility.FromJson<SerializationWrapper<int>>(sceneManagerMoneyJson);
            int[] savedSceneManagerMoney = wrappersceneManagerMoney.data;

            string sceneManagerItemsIDJson = PlayerPrefs.GetString(sceneManager.sceneName + "_ItemsID");
            SerializationWrapper<int> wrapperSceneManagerItemsID = JsonUtility.FromJson<SerializationWrapper<int>>(sceneManagerItemsIDJson);
            int[] savedSceneManagerItemsID = wrapperSceneManagerItemsID.data;

            string sceneManagerItemsNumberJson = PlayerPrefs.GetString(sceneManager.sceneName + "_ItemsNumber");
            SerializationWrapper<float> wrapperSceneManagerItemsNumber = JsonUtility.FromJson<SerializationWrapper<float>>(sceneManagerItemsNumberJson);
            float[] savedSceneManagerItemsNumber = wrapperSceneManagerItemsNumber.data;

            string sceneManagerItemsEquippedJson = PlayerPrefs.GetString(sceneManager.sceneName + "_ItemsEquipped");
            SerializationWrapper<bool> wrapperSceneManagerItemsEquipped = JsonUtility.FromJson<SerializationWrapper<bool>>(sceneManagerItemsEquippedJson);
            bool[] savedSceneManagerItemsEquipped = wrapperSceneManagerItemsEquipped.data;

            string sceneManagerItemsSellJson = PlayerPrefs.GetString(sceneManager.sceneName + "_ItemsSell");
            SerializationWrapper<bool> wrapperSceneManagerItemsSell = JsonUtility.FromJson<SerializationWrapper<bool>>(sceneManagerItemsSellJson);
            bool[] savedSceneManagerItemsSell = wrapperSceneManagerItemsSell.data;

            for (int i = 0; i < sceneManager.inventories.Length; i++)
            {
                sceneManager.inventories[i].money = savedSceneManagerMoney[i];

                for (int j = 0; j < sceneManager.inventories[i].items.Length; j++)
                {
                    if (savedSceneManagerItemsID[j + i * 25] >= 0)
                    {
                        Item newItem = Instantiate(itemsPrefab.transform.GetChild(savedSceneManagerItemsID[j + i * 25]).GetComponent<Item>(), sceneManager.inventories[i].transform);

                        //sceneManager.inventories[i].items[j] = itemsPrefab.transform.GetChild(savedSceneManagerItemsID[j + i * 25]).GetComponent<Item>();
                        newItem.ID = savedSceneManagerItemsID[j + i * 25];
                        newItem.number = savedSceneManagerItemsNumber[j + i * 25];
                        newItem.equipped = savedSceneManagerItemsEquipped[j + i * 25];
                        newItem.sell = savedSceneManagerItemsSell[j + i * 25];

                        sceneManager.inventories[i].items[j] = newItem;
                    }
                }
            }
        }
    }

    public void SaveLanguage(int LanguageIndex = -1)
    {
        if (LanguageIndex >= 0)
        {
            PlayerPrefs.SetInt("Language", LanguageIndex); // Сохраняем индекс языка
        }
        else
        {
            PlayerPrefs.SetInt("Language", (int)languageManager.currentLanguage); // Сохраняем текущий язык как индекс
        }

        PlayerPrefs.Save(); // Сохранение изменений
    }

    public void LoadLanguage()
    {
        if (PlayerPrefs.HasKey("Language"))
        {
            int savedLanguageIndex = PlayerPrefs.GetInt("Language");
            languageManager.SetLanguage((Language)savedLanguageIndex); // Восстановление языка
        }
    }

    public void LoadLanguageAsync()
    {
        // Увеличиваем счётчик активных корутин
        activeCoroutines++;
        StartCoroutine(LoadLanguageCoroutine());
    }

    private IEnumerator LoadLanguageCoroutine()
    {
        if (PlayerPrefs.HasKey("Language"))
        {
            // Асинхронная загрузка языка
            int savedLanguageIndex = PlayerPrefs.GetInt("Language");
            languageManager.SetLanguage((Language)savedLanguageIndex);

            yield return null; // Асинхронная пауза
        }

        // Уменьшаем счётчик активных корутин
        activeCoroutines--;

        // Проверяем, завершены ли все корутины
        if (activeCoroutines == 0 && loadingWindowAnimator != null)
        {
            loadingWindowAnimator.SetBool("Loading", false); // Отключаем анимацию загрузки
        }
    }

    public void Save()
    {
        saveWindowAnimator.gameObject.SetActive(false);
        saveWindowAnimator.gameObject.SetActive(true);

        // Сохранение всех данных с использованием PlayerPrefs
        SaveCharacter();
        SaveStats();
        SavePerks();
        SaveInventory();
        SaveQuests();
        SaveMap();
        SaveSettings();
        SaveScene();
        SaveTraining();

        // Сохранение сброса состояния сцены
        PlayerPrefs.SetInt(sceneManager.sceneName + "R", sceneManager.reset ? 1 : 0);

        PlayerPrefs.Save(); // Фиксация всех изменений
    }

    public void SaveToStart()
    {
        for (int i = 0; i < sceneManager.characters.Length; i++)
        {
            sceneManager.characters[i].StartCharacter();
            sceneManager.characters[i].GetComponent<HealthSystem>().StartHealth();

            if (sceneManager.deathHair[i])
            {
                sceneManager.characters[i].GetComponent<HealthSystem>().RemoveHair();
            }

            if (sceneManager.characters[i].hasLeft)
            {
                sceneManager.characters[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < sceneManager.inventories.Length; i++)
        {
            sceneManager.inventories[i].StartInventory();
        }
        for (int i = 0; i < sceneManager.works.Length; i++)
        {
            sceneManager.powerBoxes[i].StartPowerBoxes();
        }
        for (int i = 0; i < sceneManager.openDoors.Length; i++)
        {
            Door door = sceneManager.interactableObjects[i].GetComponent<Door>();

            if (sceneManager.openDoors[i])
            {
                door.Use();
            }
        }
        for (int i = 0; i < sceneManager.skills.Length; i++)
        {
            if (sceneManager.interactableObjects[i].GetComponent<Container>())
            {
                sceneManager.interactableObjects[i].GetComponent<Container>().StartContainer();
            }
        }
        if (SceneManager.GetActiveScene().name == "Dump")
        {
            for (int i = 0; i < sceneManager.repairObjects.Length; i++)
            {
                sceneManager.repairObjects[i].LoadObject(sceneManager.lastRepairObject[i], questSystem);
            }
        }
        else
        {
            for (int i = 0; i < sceneManager.repairObjects.Length; i++)
            {
                sceneManager.repairObjects[i].LoadObject(sceneManager.lastRepairObject[i]);
            }
        }

        character.UpdateHair();
        character.StartCharacter();
        character.GetComponent<HealthSystem>().StartHealth();
        character.Inventory.StartInventory();
        carInventory.StartInventory();

        character.combatSystem.targets.Clear();
        character.combatSystem.combat = false;

        character.characterMovement.canMove = true;
        Camera.main.GetComponent<CameraZoom>().OnExitObject();
        dialogueSystem.StartDialogue();
        if (dialogueSystem.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            dialogueSystem.CloseDialogueWindow();
        }

        ExperienceSystem.UpdateSlider();

        if (sceneManager.reset)
        {
            SaveCharacter();
            SaveStats();
            SavePerks();
            SaveInventory();
            SaveQuests();
            SaveMap();
            SaveSettings();
            SaveScene();
            SaveTraining();

            sceneManager.reset = false;
            PlayerPrefs.SetInt(sceneManager.sceneName + "R", 0); // Сохранение сброса состояния
            PlayerPrefs.Save(); // Фиксация изменений
        }
    }

    public void SaveToSettings()
    {
        // Сохраняем основные игровые данные
        SaveCharacter();
        SaveStats();
        SavePerks();
        SaveInventory();
        SaveQuests();
        SaveMap();
        SaveSettings();
        SaveScene();
        SaveTraining();

        // Сохраняем состояние сброса
        PlayerPrefs.SetInt(sceneManager.sceneName + "R", sceneManager.reset ? 1 : 0);

        PlayerPrefs.Save(); // Фиксация всех изменений
    }


    public void Load()
    {
        LoadQuests();
        LoadScene();
        LoadCharacter();
        LoadStats();
        LoadPerks();
        LoadInventory();
        LoadMap();
        LoadSettings();
        LoadTraining();
    }

    public void ResetInventory()
    {
        // Сбрасываем деньги в инвентаре
        character.Inventory.money = 0;

        for (int i = 0; i < character.Inventory.items.Length; i++)
        {
            character.Inventory.items[i] = null;
        }
        for (int i = 0; i < carInventory.items.Length; i++)
        {
            carInventory.items[i] = null;
        }

        // Фиксация изменений
        SaveInventory();
    }

    public void ResetStats()
    {
        // Сбрасываем основные параметры
        PlayerPrefs.SetFloat("CurrentHP", 1000); // Устанавливаем здоровье на максимум
        experienceSystem.SetXP_LVL(0, 1); // Сбрасываем опыт и уровень
        PlayerPrefs.SetFloat("XP", experienceSystem.GetCurrentXP); // Сохраняем сброшенный опыт
        PlayerPrefs.SetInt("LVL", experienceSystem.GetCurrentLVL); // Сохраняем сброшенный уровень
        PlayerPrefs.SetFloat("CurrentRadiation", 0); // Сбрасываем радиацию

        // Сбрасываем очки перков и их состояние
        character.PerkSystem.points = 0;
        for (int i = 0; i < character.PerkSystem.perks.Length; i++)
        {
            character.PerkSystem.perks[i].Active = false; // Деактивируем все перки
        }
        for (int i = 0; i < character.CharacterSkills.Length; i++)
        {
            character.CharacterSkills[i].lvlPoints = 0;
        }

        string skillsLvlPointsJson = JsonUtility.ToJson(new SerializationWrapper<int>(skillsLvlPoints));

        PlayerPrefs.SetString("SkillsLvlPoints", skillsLvlPointsJson);

        // Сохраняем состояние перков
        SavePerks();
    }

    public void ResetMap()
    {
        map.lastCell = map.Fog[83].GetComponent<Button>();
        for (int i = 0; i < fog.Length; i++)
        {
            if(i == 82 || i == 83 || i == 84 || i == 67 || i== 68 || i == 69)
            {
                map.Fog[i].find = true;
            }
            else
            {
                map.Fog[i].find = false;
            }
        }
        for (int i = 0; i < map.attendedRandomEncounter.Length; i++)
        {
            map.attendedRandomEncounter[i] = false;
        }
        SaveMap();
    }
    public void ResetQuests()
    {
        for (int i = 0; i < questSystem.quests.Length; i++)
        {
            questSystem.quests[i].Active = false;
            questSystem.quests[i].Complete = false;
            for (int j = 0; j < questSystem.quests[i].QuestParts.Length; j++)
            {
                questSystem.quests[i].QuestParts[j].Active = false;
                questSystem.quests[i].QuestParts[j].Complete = false;
            }
        }
        //Main Quest
        questSystem.quests[11].Active = true;
        questSystem.quests[11].QuestParts[0].Active = true;
        SaveQuests();
    }
    public void ResetSettings()
    {
        settings.qualityLevel = 2;
        settings.shadows = true;
        settings.musicValue = 5;
        settings.soundsValue = 10;
        QualitySettings.SetQualityLevel(2);
        SaveSettings();
    }
    public void ResetLastSceneName()
    {
        // Сбрасываем имя сцены на пустую строку
        PlayerPrefs.SetString("SceneName", "");

        // Фиксируем изменения
        PlayerPrefs.Save();
    }

    public void ResetTraining()
    {
        if(trainingSystem != null)
        {
            trainingSystem.complete = false;
            SaveTraining();
        }
    }

    public void RestartGame()
    {
        // Сброс всех данных через соответствующие методы
        ResetInventory();
        ResetStats();
        ResetLastSceneName();
        ResetMap();
        ResetQuests();
        ResetTraining();

        string str = "";
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            for (int j = 14; j < scenePath.Length; j++) // 14 — предполагаемая длина пути до имени сцены
            {
                if (scenePath[j] == '.')
                {
                    PlayerPrefs.SetInt(str + "R", 1); // Сохраняем сброс состояния сцены
                    str = "";
                    break;
                }
                str += scenePath[j];
            }
        }

        PlayerPrefs.Save(); // Фиксация всех изменений

        // Загрузка начальной сцены
        sceneManager.LoadScene("CharecterCreator");
    }
}

[System.Serializable]
public class SerializationWrapper<T>
{
    public T[] data;

    public SerializationWrapper(T[] data)
    {
        this.data = data;
    }
}