using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private SaveScript saveScript;
    [SerializeField] private LanguageManager languageManager;
    [SerializeField] private Map map;
    [SerializeField] private QuestSystem questSystem;
    [SerializeField] private GameObject lockPickingIcon;
    private Camera dialogueCamera;
    private Character player;

    [SerializeField] private Text NPCText;
    [SerializeField] private GameObject dialogueGrid;
    [SerializeField] private Button dialogueButton;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private GameObject sexObject;

    [SerializeField] private Text NPCName;
    [SerializeField] private Text PlayerName;

    private Dialogue currentDialogue;

    public bool skip;

    public void StartDialogue()
    {
        dialogueCamera = GameObject.FindGameObjectWithTag("Dialogue Camera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        PlayerName.text = player.characterName;
    }

    public void StartDialogue(GameObject parent, Dialogue dialogue)
    {
        Camera.main.GetComponent<CameraZoom>().OnPlayerPosition();
        Camera.main.GetComponent<CameraZoom>().OnPointerObject();

        if(languageManager.currentLanguage == Language.Russian)
        {
            NPCName.text = dialogue.transform.parent.GetComponent<Character>().characterName;
        }
        else if (languageManager.currentLanguage == Language.English)
        {
            NPCName.text = dialogue.transform.parent.GetComponent<Character>().engCharacterName;
        }
        else if (languageManager.currentLanguage == Language.Indonesian)
        {
            NPCName.text = dialogue.transform.parent.GetComponent<Character>().indonesianCharacterName;
        }

        currentDialogue = dialogue;

        dialogueCamera.transform.SetParent(parent.transform);
        dialogueCamera.transform.localPosition = new Vector3(0, 0, 0);
        dialogueCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);

        transform.GetChild(0).gameObject.SetActive(true);

        ShowDialogue(currentDialogue.currentNode);
    }

    void ShowDialogue(int node)
    {
        skipButton.SetActive(true);

        currentDialogue.currentNode = node;

        NPCText.text = "";

        for (int i = 0; i < dialogueGrid.transform.childCount; i++)
        {
            Destroy(dialogueGrid.transform.GetChild(i).gameObject);
        }

        if (player.male)
        {
            if (languageManager.currentLanguage == Language.Russian)
            {
                StartCoroutine(DialogueTextCoroutine(currentDialogue.nodes[currentDialogue.currentNode].maleNPCText));
            }
            else if (languageManager.currentLanguage == Language.English)
            {
                StartCoroutine(DialogueTextCoroutine(currentDialogue.nodes[currentDialogue.currentNode].engMaleNPCText));
            }
            else if (languageManager.currentLanguage == Language.Indonesian)
            {
                StartCoroutine(DialogueTextCoroutine(currentDialogue.nodes[currentDialogue.currentNode].indonesianMaleNPCText));
            }
        }
        else
        {
            if (languageManager.currentLanguage == Language.Russian)
            {
                StartCoroutine(DialogueTextCoroutine(currentDialogue.nodes[currentDialogue.currentNode].femaleNPCText));
            }
            else if (languageManager.currentLanguage == Language.English)
            {
                StartCoroutine(DialogueTextCoroutine(currentDialogue.nodes[currentDialogue.currentNode].engFemaleNPCText));
            }
            else if (languageManager.currentLanguage == Language.Indonesian)
            {
                StartCoroutine(DialogueTextCoroutine(currentDialogue.nodes[currentDialogue.currentNode].indonesianFemaleNPCText));
            }
        }
    }

    public void SkipDialogue()
    {
        skip = true;
        skipButton.SetActive(false);
    }

    void AddSound()
    {
        transform.parent.GetComponent<AudioSource>().Play();
    }

    void EndDialogue(int node, bool aggressive = false)
    {
        if (aggressive)
        {
            player.characterMovement.MoveToCombat();
        }

        player.characterMovement.canMove = true;
        Camera.main.GetComponent<CameraZoom>().OnExitObject();

        currentDialogue.currentNode = node;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void CloseDialogueWindow()
    {
        player.characterMovement.canMove = true;
        Camera.main.GetComponent<CameraZoom>().OnExitObject();

        transform.GetChild(0).gameObject.SetActive(false);
    }

    void AddXP(int xp)
    {
        ExperienceSystem.AddXP(xp);
    }
    void AddMoney(int money, bool toPlayer = true)
    {
        if (toPlayer)
        {
            player.Inventory.AddMoney(currentDialogue.transform.parent.GetComponent<Character>().Inventory, player.Inventory, money);
        }
        else
        {
            player.Inventory.AddMoney(player.Inventory, currentDialogue.transform.parent.GetComponent<Character>().Inventory, money);
        }
    }

    void StartSex()
    {
        sexObject.GetComponent<Animator>().SetTrigger("Sex");
        player.GetComponent<HealthSystem>().SexHeal();

        //googlePlayAchievements.UnlockAchievement(6);
    }

    void KillCharacter(HealthSystem healthSystem)
    {
        healthSystem.ApplyDamage(1000, Body.Torso, DamageType.Blunt);
        if (healthSystem.GetComponent<Character>().characterName == "Уставший Моряк")
        {
            //googlePlayAchievements.UnlockAchievement(5);
        }
    }

    void StartBarter()
    {
        player.Inventory.ShowInventory(true);
        currentDialogue.transform.parent.GetComponent<Character>().Inventory.ShowInventory(true);
    }

    void MinusNumber(Item item)
    {
        item.number -= 1;
        if(item.number == 0)
        {
            DestroyItem(item);
        }
    }

    void DestroyItem(Item item)
    {
        for (int i = 0; i < player.Inventory.items.Length; i++)
        {
            if (player.Inventory.items[i] != null)
            {
                if (player.Inventory.items[i].itemName == item.itemName)
                {
                    player.Inventory.items[i] = null;
                    player.GetComponent<HealthSystem>().UpdateButton();
                    return;
                }
            }
        }
    }

    void FindLocation(Fog fog)
    {
        fog.find = true;
    }

    void OpenFullMap()
    {
        map.OpenMap();
    }

    void HasLeft(Character character)
    {
        character.hasLeft = true;
    }

    void AddAttribute(string attribute)
    {
        if(attribute == "Strength")
        {
            player.Attributes.Strength += 1;
        }
        else if (attribute == "Agility")
        {
            player.Attributes.Agility += 1;
        }
        else if (attribute == "Intelligence")
        {
            player.Attributes.Intelligence += 1;
        }
        else if (attribute == "Charisma")
        {
            player.Attributes.Charisma += 1;
        }
    }

    void LoadScene(string sceneName, Fog fog)
    {
        map.PlayerMapPoint.transform.position = fog.transform.position;
        saveScript.SaveToSettings();
        saveScript.GetComponent<SceneManagerScript>().LoadScene(sceneName);
    }

    void DialogueText()
    {
        for (int i = 0; i < currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer.Length; i++)
        {
            if ((currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].answerType == AnswerType.Sex && player.male == currentDialogue.transform.parent.GetComponent<Character>().male) ||
                (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Item != null && !player.Inventory.FindItem(currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Item)) ||
                (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].NeedComplete && !questSystem.quests[currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Quest].QuestParts[currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].QuestPart].Complete) ||
                (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].DontNeedComplete && questSystem.quests[currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Quest].QuestParts[currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].QuestPart].Complete) ||
                (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Money < 0 && player.Inventory.money < Mathf.Abs(currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Money)) ||
                (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].NeedMan && player.male == false))
            {

            }
            else
            {
                Button button = Instantiate(dialogueButton, dialogueGrid.transform);
                button.onClick.AddListener(AddSound);
                Text textDialogue = button.transform.GetChild(1).GetComponent<Text>();

                Skills skill = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Skill;
                int skillCheck = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].SkillCheck;

                string attributeName = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].attributeName;

                int money = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Money;
                int xp = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].XP;

                //Speech Perk
                int dopPoints = 0;
                if (player.PerkSystem.FindPerk(Skills.Speech, 0).Active)
                {
                    if (player.male != currentDialogue.transform.parent.GetComponent<Character>().male)
                    {
                        dopPoints += 25;
                    }
                }

                textDialogue.text = "";

                if (skillCheck > 0)
                {
                    if(languageManager.currentLanguage == Language.Russian)
                    {
                        button.transform.GetChild(1).GetComponent<Text>().text = "[" + player.FindSkill(skill).ruName + " " + (player.FindSkill(skill).points + dopPoints) + "/" + skillCheck + "] ";
                    }
                    else if (languageManager.currentLanguage == Language.English)
                    {
                        button.transform.GetChild(1).GetComponent<Text>().text = "[" + player.FindSkill(skill).engName + " " + (player.FindSkill(skill).points + dopPoints) + "/" + skillCheck + "] ";
                    }
                    else if (languageManager.currentLanguage == Language.Indonesian)
                    {
                        button.transform.GetChild(1).GetComponent<Text>().text = "[" + player.FindSkill(skill).indonesianName + " " + (player.FindSkill(skill).points + dopPoints) + "/" + skillCheck + "] ";
                    }
                }

                if (player.male)
                {
                    if (languageManager.currentLanguage == Language.Russian)
                    {
                        textDialogue.text += currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Text;
                    }
                    else if (languageManager.currentLanguage == Language.English)
                    {
                        textDialogue.text += currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].EngText;
                    }
                    else if (languageManager.currentLanguage == Language.Indonesian)
                    {
                        textDialogue.text += currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].IndonesianText;
                    }
                }
                else
                {
                    if (languageManager.currentLanguage == Language.Russian)
                    {
                        textDialogue.text += currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].FemaleText;
                    }
                    else if (languageManager.currentLanguage == Language.English)
                    {
                        textDialogue.text += currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].EngFemaleText;
                    }
                    else if (languageManager.currentLanguage == Language.Indonesian)
                    {
                        textDialogue.text += currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].IndonesianFemaleText;
                    }
                }

                for (int j = 0; j < button.transform.GetChild(0).childCount; j++)
                {
                    if (button.transform.GetChild(0).GetChild(j).name == currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].answerType.ToString())
                    {
                        button.transform.GetChild(0).GetChild(j).gameObject.SetActive(true);
                        if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].answerType == AnswerType.Quest)
                        {
                            button.transform.GetChild(1).GetComponent<Text>().color = Color.yellow;
                            button.transform.GetChild(0).GetChild(j).GetComponent<Image>().color = Color.yellow;
                        }
                        else
                        {
                            button.transform.GetChild(1).GetComponent<Text>().color = Color.white;
                            button.transform.GetChild(0).GetChild(j).GetComponent<Image>().color = Color.white;
                        }
                        break;
                    }
                }

                int nextNode = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].ToNode;
                int questIndex = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Quest;
                int questPartIndex = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].QuestPart;
                Fog fog = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Fog;
                Item item = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Item;
                Door door = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Door;
                Container container = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Container;
                RepairObject repairObject = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].RepairObject;
                int nextRepairObjectIndex = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].NextRepairObjectIndex;
                string sceneName = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].sceneName;

                if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].answerType == AnswerType.Aggressive)
                {
                    button.onClick.AddListener(() => EndDialogue(nextNode, true));
                }
                else if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].answerType == AnswerType.Quest)
                {
                    if (questPartIndex < 0)
                    {
                        button.onClick.AddListener(() => questSystem.ActiveQuest(questIndex));
                    }
                    else
                    {
                        button.onClick.AddListener(() => questSystem.CompletePart(questIndex, questPartIndex));
                    }
                }
                else if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].answerType == AnswerType.Barter)
                {
                    button.onClick.AddListener(() => StartBarter());
                }
                else if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].answerType == AnswerType.Sex)
                {
                    if (skillCheck <= 0)
                    {
                        button.onClick.AddListener(() => StartSex());
                    }
                }

                if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Item != null)
                {
                    if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].destroyItem == false)
                    {
                        button.onClick.AddListener(() => currentDialogue.transform.parent.GetComponent<Character>().Inventory.AddItem(item));
                    }

                    if (player.Inventory.FindItem(item).number == 1)
                    {
                        button.onClick.AddListener(() => DestroyItem(item));
                    }
                    else
                    {
                        button.onClick.AddListener(() => MinusNumber(player.Inventory.FindItem(item)));
                    }

                    if(item.itemType == ItemType.Medkit || item.itemType == ItemType.Grenade)
                    {
                        button.onClick.AddListener(() => player.GetComponent<HealthSystem>().UpdateButton());
                        button.onClick.AddListener(() => player.GetComponent<GrenadeUIManager>().UpdateButton());
                    }
                }
                if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Door != null)
                {
                    lockPickingIcon.SetActive(false);
                    lockPickingIcon.SetActive(true);
                    button.onClick.AddListener(() => door.OpenDoor());
                }
                if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Container != null)
                {
                    lockPickingIcon.SetActive(false);
                    lockPickingIcon.SetActive(true);
                    button.onClick.AddListener(() => container.OpenContainer());
                }
                if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].RepairObject != null)
                {
                    button.onClick.AddListener(() => repairObject.NextRepairObjectDialogue(nextRepairObjectIndex));
                }
                if (money > 0)
                {
                    button.onClick.AddListener(() => AddMoney(money));
                }
                else if (money < 0)
                {
                    money = Mathf.Abs(money);
                    button.onClick.AddListener(() => AddMoney(money, false));
                }
                if(xp > 0)
                {
                    button.onClick.AddListener(() => AddXP(xp));
                }
                if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].kill)
                {
                    button.onClick.AddListener(() => KillCharacter(currentDialogue.transform.parent.GetComponent<HealthSystem>()));
                }
                if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].hasLeft)
                {
                    button.onClick.AddListener(() => HasLeft(currentDialogue.transform.parent.GetComponent<Character>()));
                }
                if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].Fog != null)
                {
                    button.onClick.AddListener(() => FindLocation(fog));
                }
                if (sceneName != "" && fog != null)
                {
                    button.onClick.AddListener(() => EndDialogue(nextNode));
                    button.onClick.AddListener(() => LoadScene(sceneName, fog));
                }
                if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].openFullMap)
                {
                    button.onClick.AddListener(() => OpenFullMap());
                }

                if (currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].answerType == AnswerType.SpeakEnd)
                {
                    button.onClick.AddListener(() => EndDialogue(nextNode));
                }
                else
                {
                    if (skillCheck > 0)
                    {
                        if (player.FindSkill(skill).points + dopPoints >= skillCheck)
                        {
                            button.onClick.AddListener(() => ShowDialogue(nextNode));
                            button.onClick.AddListener(() => AddXP(25));
                        }
                        else
                        {
                            int failNode = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].FailNode;
                            button.onClick.AddListener(() => ShowDialogue(failNode));
                        }
                    }
                    else if(attributeName != "")
                    {
                        if ((attributeName == "Strength" && player.Attributes.Strength <= 9) ||
                            (attributeName == "Agility" && player.Attributes.Agility <= 9) ||
                            (attributeName == "Intelligence" && player.Attributes.Intelligence <= 9) ||
                            (attributeName == "Charisma" && player.Attributes.Charisma <= 9))
                        {
                            button.onClick.AddListener(() => ShowDialogue(nextNode));
                            button.onClick.AddListener(() => AddAttribute(attributeName));
                        }
                        else
                        {
                            int failNode = currentDialogue.nodes[currentDialogue.currentNode].PlayerAnswer[i].FailNode;
                            button.onClick.AddListener(() => ShowDialogue(failNode));
                        }
                    }
                    else
                    {
                        button.onClick.AddListener(() => ShowDialogue(nextNode));
                    }
                }
            }
        }
    }

    IEnumerator DialogueTextCoroutine(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (skip)
            {
                NPCText.text = text;
                skip = false;
                break;
            }
            yield return new WaitForSeconds(0.01f);

            NPCText.text += text[i];
        }
        skipButton.SetActive(false);
        DialogueText();
        StopCoroutine(DialogueTextCoroutine(text));
    }
}