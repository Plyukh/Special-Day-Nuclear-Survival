using UnityEngine;
using UnityEngine.SceneManagement;

public class RepairObject : Interactable
{
    [SerializeField] private GameObject[] nextRepairObject;

    [SerializeField] private Item[] items;

    [SerializeField] private int quest;
    [SerializeField] private int questPart;

    [SerializeField] private Dialogue person;
    public int toPersonNode;

    [SerializeField] public GameObject[] allObjects;

    protected new void OnEnable()
    {
        base.OnEnable();
        for (int i = 0; i < allObjects.Length; i++)
        {
            allObjects[i].transform.GetChild(0).GetComponent<RepairObject>().person = person;
        }
    }

    public void NextRepairObject(GameObject repairObject, Item item = null, bool FindQuest = true)
    {
        GameObject repair_object = Instantiate(repairObject, transform.parent.transform.position, transform.parent.transform.rotation, transform.parent.parent.transform);
        repair_object.transform.GetChild(0).GetComponent<RepairObject>().audioSource.Play();

        gameObject.transform.parent.gameObject.SetActive(false);

        if(person != null)
        {
            person.currentNode = repairObject.transform.GetChild(0).GetComponent<RepairObject>().toPersonNode;
        }

        if (quest > -1 && FindQuest)
        {
            QuestSystem questSystem = FindFirstObjectByType<QuestSystem>();
            if(item != null)
            {
                for (int i = 0; i < questSystem.quests[quest].QuestParts.Length; i++)
                {
                    if (questSystem.quests[quest].QuestParts[i].QuestType == QuestType.Item && questSystem.quests[quest].QuestParts[i].Complete == false)
                    {
                        if (questSystem.Player.Inventory.FindItem(questSystem.quests[quest].QuestParts[i].Item))
                        {
                            for (int j = 0; j < questSystem.Player.Inventory.items.Length; j++)
                            {
                                if(questSystem.Player.Inventory.items[j] != null)
                                {
                                    if (questSystem.Player.Inventory.items[j].itemName == item.itemName)
                                    {
                                        questSystem.CompletePart(quest,i);
                                        questSystem.Player.Inventory.items[j] = null;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                questSystem.CompletePart(quest, questPart);
            }
        }
    }

    public void NextRepairObjectDialogue(int x)
    {
        for (int j = 0; j < FindFirstObjectByType<SceneManagerScript>().repairObjects.Length; j++)
        {
            if (FindFirstObjectByType<SceneManagerScript>().repairObjects[j].transform.parent.name == allObjects[0].name)
            {
                FindFirstObjectByType<SceneManagerScript>().lastRepairObject[j] = allObjects[x].name;
                break;
            }
        }

        NextRepairObject(allObjects[x], null, false);
    }

    public void LoadObject(string LastObjectName, QuestSystem questSystem = null)
    {
        if(questSystem != null)
        {
            if (SceneManager.GetActiveScene().name == "Dump" && questSystem.quests[13].Complete)
            {
                transform.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < allObjects.Length; i++)
            {
                if (allObjects[i].name == LastObjectName)
                {
                    Instantiate(allObjects[i], transform.parent.transform.position, transform.parent.transform.rotation, transform.parent.parent.transform);
                    gameObject.transform.parent.gameObject.SetActive(false);
                    return;
                }
            }
        }
    }

    public override void UseSkill(Skill CurrentSkill)
    {
        if (needSkill != null)
        {
            if (CurrentSkill.skill == needSkill.skill)
            {
                int points = CurrentSkill.points;

                if(items.Length > 0)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (points >= needSkill.points && CurrentSkill.transform.parent.parent.GetComponent<Character>().Inventory.FindItem(items[i]))
                        {
                            for (int j = 0; j < FindFirstObjectByType<SceneManagerScript>().repairObjects.Length; j++)
                            {
                                if(FindFirstObjectByType<SceneManagerScript>().repairObjects[j].transform.parent.name == allObjects[0].name)
                                {
                                    FindFirstObjectByType<SceneManagerScript>().lastRepairObject[j] = nextRepairObject[i].name;
                                    break;
                                }
                            }

                            if (languageManager.currentLanguage == Language.Russian)
                            {
                                EventLog.Print("Применение навыка \"" + CurrentSkill.ruName + "\" прошел успешно", Color.green);
                            }
                            else if (languageManager.currentLanguage == Language.English)
                            {
                                EventLog.Print("The skill \"" + CurrentSkill.engName + "\" was successfully applied", Color.green);
                            }
                            else if (languageManager.currentLanguage == Language.Indonesian)
                            {
                                EventLog.Print("Keterampilan \"" + CurrentSkill.indonesianName + "\" berhasil diterapkan", Color.green);
                            }
                            ExperienceSystem.AddXP(25);
                            NextRepairObject(nextRepairObject[i], items[i]);
                            return;
                        }
                        else if (i == items.Length - 1 && points >= needSkill.points)
                        {
                            if (languageManager.currentLanguage == Language.Russian)
                            {
                                EventLog.Print("Отсутствует предмет для починки", Color.red);
                            }
                            else if (languageManager.currentLanguage == Language.English)
                            {
                                EventLog.Print("Missing item to repair", Color.red);
                            }
                            else if (languageManager.currentLanguage == Language.Indonesian)
                            {
                                EventLog.Print("Item yang hilang untuk diperbaiki", Color.red);
                            }
                            return;
                        }
                        else if (points < needSkill.points)
                        {
                            if (languageManager.currentLanguage == Language.Russian)
                            {
                                EventLog.Print("Недостаточно навыка", Color.red);
                            }
                            else if (languageManager.currentLanguage == Language.English)
                            {
                                EventLog.Print("Not enough skill", Color.red);
                            }
                            else if (languageManager.currentLanguage == Language.Indonesian)
                            {
                                EventLog.Print("Tidak cukup keterampilan", Color.red);
                            }
                            return;
                        }
                    }
                }
                else
                {
                    if (points >= needSkill.points)
                    {
                        for (int j = 0; j < FindFirstObjectByType<SceneManagerScript>().repairObjects.Length; j++)
                        {
                            if (FindFirstObjectByType<SceneManagerScript>().repairObjects[j].transform.parent.name == allObjects[0].name)
                            {
                                FindFirstObjectByType<SceneManagerScript>().lastRepairObject[j] = nextRepairObject[1].name;
                                break;
                            }
                        }
                        if (languageManager.currentLanguage == Language.Russian)
                        {
                            EventLog.Print("Применение навыка \"" + CurrentSkill.ruName + "\" прошел успешно", Color.green);
                        }
                        else if (languageManager.currentLanguage == Language.English)
                        {
                            EventLog.Print("The skill \"" + CurrentSkill.engName + "\" was successfully applied", Color.green);
                        }
                        else if (languageManager.currentLanguage == Language.Indonesian)
                        {
                            EventLog.Print("Keterampilan \"" + CurrentSkill.indonesianName + "\" berhasil diterapkan", Color.green);
                        }
                        ExperienceSystem.AddXP(25);
                        NextRepairObject(nextRepairObject[1]);
                        return;
                    }
                    else
                    {
                        if (languageManager.currentLanguage == Language.Russian)
                        {
                            EventLog.Print("Недостаточно навыка", Color.red);
                        }
                        else if (languageManager.currentLanguage == Language.English)
                        {
                            EventLog.Print("Not enough skill", Color.red);
                        }
                        else if (languageManager.currentLanguage == Language.Indonesian)
                        {
                            EventLog.Print("Tidak cukup keterampilan", Color.red);
                        }
                    }
                }
            }
            else
            {
                if (languageManager.currentLanguage == Language.Russian)
                {
                    EventLog.Print("Ничего не происходит", Color.red);
                }
                else if (languageManager.currentLanguage == Language.English)
                {
                    EventLog.Print("Nothing happens", Color.red);
                }
                else if (languageManager.currentLanguage == Language.Indonesian)
                {
                    EventLog.Print("Tidak terjadi apa-apa", Color.red);
                }
            }
        }
        else
        {
            if (languageManager.currentLanguage == Language.Russian)
            {
                EventLog.Print("Ничего не происходит", Color.red);
            }
            else if (languageManager.currentLanguage == Language.English)
            {
                EventLog.Print("Nothing happens", Color.red);
            }
            else if (languageManager.currentLanguage == Language.Indonesian)
            {
                EventLog.Print("Tidak terjadi apa-apa", Color.red);
            }
        }
    }

    public override void Use()
    {
        if (languageManager.currentLanguage == Language.Russian)
        {
            EventLog.Print("Вы уже починили это", Color.green);
        }
        else if (languageManager.currentLanguage == Language.English)
        {
            EventLog.Print("You've already fixed it", Color.green);
        }
        else if (languageManager.currentLanguage == Language.Indonesian)
        {
            EventLog.Print("Kamu sudah memperbaikinya", Color.green);
        }
    }
}
