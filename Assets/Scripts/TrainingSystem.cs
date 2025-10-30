using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingSystem : MonoBehaviour
{
    [SerializeField] private Character player;
    [SerializeField] private LanguageManager languageManager;
    [SerializeField] private QuestSystem questSystem;
    [SerializeField] private Map map;
    [SerializeField] private SaveScript saveScript;
    [HideInInspector] public bool complete;

    [SerializeField] private Blueprint[] blueprints;
    [SerializeField] private Button[] panelCraftButtons;
    [SerializeField] private Button[] panelStatsButton;
    [SerializeField] private Button[] panelQuestButton;

    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button stealthButton;
    [SerializeField] private Button grenadeButton;
    [SerializeField] private Button healButton;

    [SerializeField] private Button craftButton;
    [SerializeField] private Button characterButton;
    [SerializeField] private Button questButton;
    [SerializeField] private Button settingButton;

    [SerializeField] private Button attackButton;
    [SerializeField] private Button dialogueButton;
    [SerializeField] private Button useButton;

    [SerializeField] private DragAndDrop dragAndDrop;

    [SerializeField] private Button playerDialoguePrefab;

    [SerializeField] private Button closeInventoryButton;
    [SerializeField] private Button closeCraftButton;
    [SerializeField] private Button closeStatsButton;
    [SerializeField] private Button closeQuestButton;

    [SerializeField] private Button nextButton;

    [SerializeField] private Text descriptionText;

    [SerializeField] private Interactable[] interactableObjects;

    [SerializeField] private TrainingPart[] trainingParts;

    private int currentDescriptionIndex = -1;

    private void Start()
    {
        NextDescription();

        inventoryButton.interactable = false;
        stealthButton.interactable = false;
        grenadeButton.interactable = false;
        healButton.interactable = false;

        craftButton.interactable = false;
        characterButton.interactable = false;
        questButton.interactable = false;
        settingButton.interactable = false;

        if (trainingParts[0].interactableObjectIndexActiveF.Length > 0)
        {
            for (int x = 0; x < trainingParts[0].interactableObjectIndexActiveF.Length; x++)
            {
                if (interactableObjects[trainingParts[0].interactableObjectIndexActiveF[x]].Outline != null)
                {
                    interactableObjects[trainingParts[0].interactableObjectIndexActiveF[x]].Outline.enabled = false;
                }
                else
                {
                    for (int y = 0; y < interactableObjects[trainingParts[0].interactableObjectIndexActiveF[x]].transform.parent.parent.parent.parent.parent.parent.GetChild(5).childCount; y++)
                    {
                        interactableObjects[trainingParts[0].interactableObjectIndexActiveF[x]].transform.parent.parent.parent.parent.parent.parent.GetChild(5).GetChild(y).GetComponent<Outline>().OutlineWidth = 0;
                    }
                }
                interactableObjects[trainingParts[0].interactableObjectIndexActiveF[x]].training = true;
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < trainingParts.Length; i++)
        {
            if(trainingParts[i].Active && !trainingParts[i].Complete)
            {
                if (trainingParts[i].Room != null)
                {
                    if (player.characterMovement.CurrentRoom.name == trainingParts[i].Room.name)
                    {
                        if (i == trainingParts.Length - 1)
                        {
                            CompleteTraining(true);
                            return;
                        }
                        CompletePart(i);
                    }
                }
                else if(trainingParts[i].inventoryOpen)
                {
                    if (player.Inventory.armorSlot.transform.parent.parent.parent.gameObject.activeInHierarchy == true)
                    {
                        CompletePart(i);
                        if (player.Inventory.armorSlot.transform.parent.parent.parent.GetChild(1).gameObject.activeInHierarchy)
                        {
                            GetComponent<RectTransform>().anchoredPosition += new Vector2(-450,125);
                        }
                    }
                }
                else if (trainingParts[i].inventoryClose)
                {
                    if (player.Inventory.armorSlot.transform.parent.parent.parent.gameObject.activeInHierarchy == false)
                    {
                        CompletePart(i);
                        if (GetComponent<RectTransform>().anchoredPosition != new Vector2(0, 125))
                        {
                            GetComponent<RectTransform>().anchoredPosition -= new Vector2(-450, 125);
                        }
                    }
                }
                else if (trainingParts[i].mapOpen)
                {
                    if (panelQuestButton[0].transform.parent.parent.GetChild(2).gameObject.activeInHierarchy == true)
                    {
                        CompletePart(i);
                        GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 450);
                    }
                }
                else if (trainingParts[i].mapClose)
                {
                    if (panelQuestButton[0].transform.parent.parent.GetChild(2).gameObject.activeInHierarchy == false)
                    {
                        CompletePart(i);
                        if (GetComponent<RectTransform>().anchoredPosition != new Vector2(0, 125))
                        {
                            GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 450);
                        }
                    }
                }
                else if (trainingParts[i].dialogueOpen)
                {
                    if (transform.parent.GetChild(9).GetChild(0).gameObject.activeInHierarchy == true)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].dialogueClose)
                {
                    if (transform.parent.GetChild(9).GetChild(0).gameObject.activeInHierarchy == false)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].craftingOpen)
                {
                    if (transform.parent.GetChild(12).GetChild(0).gameObject.activeInHierarchy == true)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].craftingClose)
                {
                    if (transform.parent.GetChild(12).GetChild(0).gameObject.activeInHierarchy == false)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].statsOpen)
                {
                    if (transform.parent.GetChild(10).GetChild(0).gameObject.activeInHierarchy == true)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].statsClose)
                {
                    if (transform.parent.GetChild(10).GetChild(0).gameObject.activeInHierarchy == false)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].perksOpen)
                {
                    if (transform.parent.GetChild(10).GetChild(0).GetChild(2).gameObject.activeInHierarchy == true)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].perksClose)
                {
                    if (transform.parent.GetChild(10).GetChild(0).GetChild(2).gameObject.activeInHierarchy == false)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].questOpen)
                {
                    if (transform.parent.GetChild(8).GetChild(0).gameObject.activeInHierarchy == true)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].questClose)
                {
                    if (transform.parent.GetChild(8).GetChild(0).gameObject.activeInHierarchy == false)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].mapOpen)
                {
                    if (transform.parent.GetChild(8).GetChild(0).GetChild(2).gameObject.activeInHierarchy == true)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].mapClose)
                {
                    if (transform.parent.GetChild(8).GetChild(0).GetChild(2).gameObject.activeInHierarchy == false)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].stealthOn)
                {
                    if (player.stealthSystem.stealth == true)
                    {
                        player.stealthSystem.training = true;
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].stealthOff)
                {
                    if (player.stealthSystem.stealth == false)
                    {
                        player.stealthSystem.training = false;
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].combatOn)
                {
                    if (player.combatSystem.combat == true)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].combatOff)
                {
                    if (player.combatSystem.combat == false)
                    {
                        player.combatSystem.training = false;
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].items.Length > 0)
                {
                    int x = 0;
                    for (int j = 0; j < trainingParts[i].items.Length; j++)
                    {
                        if (player.Inventory.FindItem(trainingParts[i].items[j]))
                        {
                            x += 1;
                        }
                    }
                    if(x == trainingParts[i].items.Length)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].checkGrenade_checkHeal)
                {
                    if(player.Inventory.grenadeSlot.item != null && player.Inventory.medicineSlot.item != null)
                    {
                        CompletePart(i);
                    }
                }
                else if (trainingParts[i].checkUseHeal)
                {
                    if (player.Inventory.medicineSlot.item == null)
                    {
                        CompletePart(i);
                    }
                }
                break;
            }
        }
    }

    public void NextDescription()
    {
        for (int i = 0; i < trainingParts.Length; i++)
        {
            if (trainingParts[i].Active && !trainingParts[i].Complete)
            {
                for (int j = 0; j < trainingParts[i].Description.Length; j++)
                {
                    if(i == 0 && j == 1)
                    {
                        questSystem.ActiveQuest(12);
                    }

                    if (j +1 != trainingParts[i].Description.Length - 1 && !nextButton.gameObject.activeInHierarchy)
                    {
                        nextButton.gameObject.SetActive(true);
                    }

                    //First Description
                    if (j == 0 && currentDescriptionIndex < 0)
                    {
                        player.characterMovement.canMove = false;

                        if (trainingParts[i].dragAndDropOff)
                        {
                            dragAndDrop.training = true;
                        }
                        else if (trainingParts[i].dragAndDropOn)
                        {
                            dragAndDrop.training = false;
                        }

                        if (trainingParts[i].closeInventoryButtonOn)
                        {
                            closeInventoryButton.interactable = true;
                        }
                        else if (trainingParts[i].closeInventoryButtonOff)
                        {
                            closeInventoryButton.interactable = false;
                        }

                        if (trainingParts[i].attackButtonOn)
                        {
                            attackButton.interactable = true;
                        }
                        else if (trainingParts[i].attackButtonOff)
                        {
                            attackButton.interactable = false;
                        }

                        if (trainingParts[i].dialogueButtonOn)
                        {
                            dialogueButton.interactable = true;
                        }
                        else if (trainingParts[i].dialogueButtonOff)
                        {
                            dialogueButton.interactable = false;
                        }

                        if (trainingParts[i].stealthButtonOn)
                        {
                            stealthButton.interactable = true;
                        }
                        else if (trainingParts[i].stealthButtonOff)
                        {
                            stealthButton.interactable = false;
                        }

                        if (trainingParts[i].useButtonOn)
                        {
                            useButton.interactable = true;
                        }
                        else if (trainingParts[i].useButtonOff)
                        {
                            useButton.interactable = false;
                        }

                        if (trainingParts[i].inventoryButtonOn)
                        {
                            inventoryButton.interactable = true;
                        }
                        else if (trainingParts[i].inventoryButtonOff)
                        {
                            inventoryButton.interactable = false;
                        }

                        if (trainingParts[i].crafingBlueprintsOff)
                        {
                            TrainingCrafting(false);
                        }

                        if (trainingParts[i].grenadeButtonOff)
                        {
                            grenadeButton.interactable = false;
                        }

                        if (trainingParts[i].healButtonOff)
                        {
                            healButton.interactable = false;
                        }

                        if (trainingParts[i].stealTrainingOff)
                        {
                            dragAndDrop.trainingSteal = false;
                        }

                        if (trainingParts[i].checkEnemyHealth)
                        {
                            player.combatSystem.training = true;
                            player.combatSystem.Target().GetComponent<CombatSystem>().training = true;
                        }

                        if (trainingParts[i].dialogueClose)
                        {
                            playerDialoguePrefab.interactable = false;
                        }

                        if(languageManager.currentLanguage == Language.Russian)
                        {
                            descriptionText.text = trainingParts[i].Description[0];
                        }
                        else if(languageManager.currentLanguage == Language.English)
                        {
                            descriptionText.text = trainingParts[i].EngDescription[0];
                        }
                        else if (languageManager.currentLanguage == Language.Indonesian)
                        {
                            descriptionText.text = trainingParts[i].IndonesianDescription[0];
                        }
                        currentDescriptionIndex = j;
                        return;
                    }
                    else if (descriptionText.text == trainingParts[i].Description[j] || descriptionText.text == trainingParts[i].EngDescription[j] || descriptionText.text == trainingParts[i].IndonesianDescription[j])
                    {
                        // Last Description
                        if (j + 1 == trainingParts[i].Description.Length - 1)
                        {
                            nextButton.gameObject.SetActive(false);

                            player.characterMovement.canMove = true;

                            if (trainingParts[i].dragAndDropEndOn)
                            {
                                dragAndDrop.training = false;
                            }
                            else if (trainingParts[i].dragAndDropEndOff)
                            {
                                dragAndDrop.training = true;
                            }

                            if (trainingParts[i].closeInventoryButtonEndOn)
                            {
                                closeInventoryButton.interactable = true;
                                closeInventoryButton.GetComponent<Animator>().SetBool("Training", true);
                            }
                            else if (trainingParts[i].closeInventoryButtonEndOff)
                            {
                                closeInventoryButton.interactable = false;
                            }

                            if (trainingParts[i].attackButtonEndOn)
                            {
                                attackButton.interactable = true;
                            }
                            else if (trainingParts[i].attackButtonEndOff)
                            {
                                attackButton.interactable = false;
                            }

                            if (trainingParts[i].dialogueButtonEndOn)
                            {
                                dialogueButton.interactable = true;
                            }
                            else if (trainingParts[i].dialogueButtonEndOff)
                            {
                                dialogueButton.interactable = false;
                            }

                            if (trainingParts[i].stealthButtonEndOn)
                            {
                                stealthButton.interactable = true;
                                stealthButton.GetComponent<Animator>().SetBool("Training", true);
                            }
                            else if (trainingParts[i].stealthButtonEndOff)
                            {
                                stealthButton.interactable = false;
                            }

                            if (trainingParts[i].useButtonEndOn)
                            {
                                useButton.interactable = true;
                            }
                            else if (trainingParts[i].useButtonEndOff)
                            {
                                useButton.interactable = false;
                            }

                            if (trainingParts[i].inventoryButtonEndOn)
                            {
                                inventoryButton.interactable = true;
                                inventoryButton.GetComponent<Animator>().SetBool("Training", true);
                            }
                            else if (trainingParts[i].inventoryButtonEndOff)
                            {
                                inventoryButton.interactable = false;
                            }

                            if (trainingParts[i].crafingBlueprintsOn)
                            {
                                TrainingCrafting(true);
                            }
                            else if (trainingParts[i].crafingBlueprintsOff)
                            {
                                TrainingCrafting(false);
                            }

                            if (trainingParts[i].perksOn)
                            {
                                TrainingPerks(true);
                            }
                            else if (trainingParts[i].perksOff)
                            {
                                TrainingPerks(false);
                            }

                            if (trainingParts[i].mapOn)
                            {
                                TrainingMap(true);
                            }
                            else if (trainingParts[i].mapOff)
                            {
                                TrainingMap(false);
                            }

                            if (trainingParts[i].grenadeButtonEndOn)
                            {
                                grenadeButton.interactable = true;
                            }
                            if (trainingParts[i].healButtonEndOn)
                            {
                                healButton.interactable = true;
                            }

                            if (trainingParts[i].craftButtonEndOn)
                            {
                                craftButton.interactable = true;
                                craftButton.GetComponent<Animator>().SetBool("Training", true);
                            }
                            else if (trainingParts[i].craftButtonEndOff)
                            {
                                craftButton.interactable = false;
                            }
                            if (trainingParts[i].closeCraftButtonEndOn)
                            {
                                closeCraftButton.interactable = true;
                                closeCraftButton.GetComponent<Animator>().SetBool("Training", true);
                            }
                            else if (trainingParts[i].closeCraftButtonEndOff)
                            {
                                closeCraftButton.interactable = false;
                            }

                            if (trainingParts[i].statsButtonEndOn)
                            {
                                characterButton.interactable = true;
                                characterButton.GetComponent<Animator>().SetBool("Training", true);
                            }
                            else if (trainingParts[i].statsButtonEndOff)
                            {
                                characterButton.interactable = false;
                            }
                            if (trainingParts[i].closeStatsButtonEndOn)
                            {
                                closeStatsButton.interactable = true;
                                closeStatsButton.GetComponent<Animator>().SetBool("Training", true);
                            }
                            else if (trainingParts[i].closeStatsButtonEndOff)
                            {
                                closeStatsButton.interactable = false;
                            }

                            if (trainingParts[i].questButtonEndOn)
                            {
                                questButton.interactable = true;
                                questButton.GetComponent<Animator>().SetBool("Training", true);
                            }
                            else if (trainingParts[i].questButtonEndOff)
                            {
                                questButton.interactable = false;
                            }
                            if (trainingParts[i].closeQuestButtonEndOn)
                            {
                                closeQuestButton.interactable = true;
                                closeQuestButton.GetComponent<Animator>().SetBool("Training", true);
                            }
                            else if (trainingParts[i].closeQuestButtonEndOff)
                            {
                                closeQuestButton.interactable = false;
                            }

                            if (trainingParts[i].stealTrainingOn)
                            {
                                dragAndDrop.trainingSteal = true;
                            }

                            if (trainingParts[i].dialogueClose)
                            {
                                playerDialoguePrefab.interactable = true;
                                transform.parent.GetChild(9).GetChild(0).GetChild(3).GetChild(0).GetComponent<Button>().interactable = true;
                            }

                            if (trainingParts[i].interactableObjectIndexActiveT.Length > 0)
                            {
                                for (int x = 0; x < trainingParts[i].interactableObjectIndexActiveT.Length; x++)
                                {
                                    if (interactableObjects[trainingParts[i].interactableObjectIndexActiveT[x]].Outline != null)
                                    {
                                        interactableObjects[trainingParts[i].interactableObjectIndexActiveT[x]].Outline.enabled = true;
                                    }
                                    else
                                    {
                                        for (int y = 0; y < interactableObjects[trainingParts[i].interactableObjectIndexActiveT[x]].transform.parent.parent.parent.parent.parent.parent.GetChild(5).childCount; y++)
                                        {
                                            interactableObjects[trainingParts[i].interactableObjectIndexActiveT[x]].transform.parent.parent.parent.parent.parent.parent.GetChild(5).GetChild(y).GetComponent<Outline>().OutlineWidth = 2;
                                        }
                                    }
                                    interactableObjects[trainingParts[i].interactableObjectIndexActiveT[x]].training = false;
                                }
                            }
                            if (trainingParts[i].interactableObjectIndexActiveF.Length > 0)
                            {
                                for (int x = 0; x < trainingParts[i].interactableObjectIndexActiveF.Length; x++)
                                {
                                    if(interactableObjects[trainingParts[i].interactableObjectIndexActiveF[x]].Outline != null)
                                    {
                                        interactableObjects[trainingParts[i].interactableObjectIndexActiveF[x]].Outline.enabled = false;
                                    }
                                    else
                                    {
                                        for (int y = 0; y < interactableObjects[trainingParts[i].interactableObjectIndexActiveF[x]].transform.parent.parent.parent.parent.parent.parent.GetChild(5).childCount; y++)
                                        {
                                            interactableObjects[trainingParts[i].interactableObjectIndexActiveF[x]].transform.parent.parent.parent.parent.parent.parent.GetChild(5).GetChild(y).GetComponent<Outline>().OutlineWidth = 0;
                                        }
                                    }
                                    interactableObjects[trainingParts[i].interactableObjectIndexActiveF[x]].training = true;
                                }
                            }
                        }

                        if(i == trainingParts.Length - 1 && j == trainingParts[i].Description.Length - 1)
                        {
                            return;
                        }
                        else
                        {
                            if (languageManager.currentLanguage == Language.Russian)
                            {
                                descriptionText.text = trainingParts[i].Description[currentDescriptionIndex + 1];
                            }
                            else if (languageManager.currentLanguage == Language.English)
                            {
                                descriptionText.text = trainingParts[i].EngDescription[currentDescriptionIndex + 1];
                            }
                            else if (languageManager.currentLanguage == Language.Indonesian)
                            {
                                descriptionText.text = trainingParts[i].IndonesianDescription[currentDescriptionIndex + 1];
                            }
                            currentDescriptionIndex = j + 1;
                            return;
                        }
                    }
                }
            }
        }
    }

    public void CompletePart(int index)
    {
        if (index != trainingParts.Length - 1)
        {
            trainingParts[index + 1].Active = true;
        }
        trainingParts[index].Complete = true;
        currentDescriptionIndex = -1;
        NextDescription();
    }

    public void CompleteTraining(bool CompleteQuest)
    {
        complete = true;

        // Завершение части квеста, если нужно
        if (CompleteQuest)
        {
            questSystem.CompletePart(12, 0);
        }

        // Разрешить передвижение игрока
        player.characterMovement.canMove = true;

        // Сброс состояний тренировки
        dragAndDrop.training = false;
        dragAndDrop.trainingSteal = false;

        // Включение интерактивности кнопок
        var buttons = new[]
        {
        inventoryButton, stealthButton, craftButton, characterButton,
        questButton, settingButton, attackButton, dialogueButton,
        useButton, closeCraftButton, closeInventoryButton,
        closeQuestButton, closeStatsButton
    };

        foreach (var button in buttons)
        {
            button.interactable = true;
        }

        // Обработка интерактивных объектов
        foreach (var obj in interactableObjects)
        {
            if (obj.Outline != null)
            {
                obj.Outline.enabled = true;
            }
            else if (CompleteQuest)
            {
                var parent = obj.transform.parent?.parent?.parent?.parent?.parent?.parent;
                if (parent != null)
                {
                    var outlineParent = parent.GetChild(5);
                    for (int i = 0; i < outlineParent.childCount; i++)
                    {
                        var outline = outlineParent.GetChild(i).GetComponent<Outline>();
                        if (outline != null)
                        {
                            outline.OutlineWidth = 2;
                        }
                    }
                }
            }

            obj.training = false;
        }

        // Сохранение и отключение объекта
        saveScript.Save();
        gameObject.SetActive(false);
    }

    public void TrainingCrafting(bool value)
    {
        if (value)
        {
            for (int i = 0; i < panelCraftButtons.Length; i++)
            {
                if(i == 0)
                {
                    panelCraftButtons[i].interactable = true;
                    panelCraftButtons[i].GetComponent<Animator>().SetBool("Training", true);
                }
                else
                {
                    panelCraftButtons[i].interactable = false;
                }
            }
            for (int i = 0; i < blueprints.Length; i++)
            {
                if(i == 0)
                {
                    blueprints[i].needScienceSkill = 0;
                }
                else
                {
                    blueprints[i].GetComponent<Button>().interactable = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < panelCraftButtons.Length; i++)
            {
                panelCraftButtons[i].interactable = true;
            }
            for (int i = 0; i < blueprints.Length; i++)
            {
                if (i == 0)
                {
                    blueprints[i].needScienceSkill = 25;
                }
                else
                {
                    blueprints[i].GetComponent<Button>().interactable = true;
                }
            }
        }
    }
    public void TrainingPerks(bool value)
    {
        if (value)
        {
            for (int i = 0; i < panelStatsButton.Length; i++)
            {
                if (i == 1)
                {
                    panelStatsButton[i].interactable = true;
                    panelStatsButton[i].GetComponent<Animator>().SetBool("Training", true);
                }
                else
                {
                    panelStatsButton[i].interactable = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < panelStatsButton.Length; i++)
            {
                panelStatsButton[i].interactable = true;
            }
        }
    }
    public void TrainingMap(bool value)
    {
        if (value)
        {
            for (int i = 0; i < panelQuestButton.Length; i++)
            {
                if (i == 1)
                {
                    panelQuestButton[i].interactable = true;
                    panelQuestButton[i].GetComponent<Animator>().SetBool("Training", true);
                }
                else
                {
                    panelQuestButton[i].interactable = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < panelQuestButton.Length; i++)
            {
                panelQuestButton[i].interactable = true;
            }
        }
    }

    public void SetTrainingBoolFalse(Animator animator)
    {
        animator.SetBool("Training", false);
    }

    [System.Serializable]
    public class TrainingPart
    {
        public string[] Description;
        public string[] EngDescription;
        public string[] IndonesianDescription;

        public Room Room;

        public int[] interactableObjectIndexActiveT;
        public int[] interactableObjectIndexActiveF;

        public bool stealthOn;
        public bool stealthOff;

        public bool combatOn;
        public bool combatOff;

        public bool inventoryOpen;
        public bool inventoryClose;

        public bool dialogueOpen;
        public bool dialogueClose;

        public bool craftingOpen;
        public bool craftingClose;

        public bool statsOpen;
        public bool statsClose;

        public bool perksOpen;
        public bool perksClose;

        public bool questOpen;
        public bool questClose;

        public bool mapOpen;
        public bool mapClose;

        public bool crafingBlueprintsOn;
        public bool crafingBlueprintsOff;

        public bool perksOn;
        public bool perksOff;

        public bool mapOn;
        public bool mapOff;

        public bool closeInventoryButtonOn;
        public bool closeInventoryButtonOff;

        public bool closeInventoryButtonEndOn;
        public bool closeInventoryButtonEndOff;

        public bool closeCraftButtonEndOn;
        public bool closeCraftButtonEndOff;

        public bool closeStatsButtonEndOn;
        public bool closeStatsButtonEndOff;

        public bool closeQuestButtonEndOn;
        public bool closeQuestButtonEndOff;

        public bool attackButtonOn;
        public bool attackButtonOff;

        public bool attackButtonEndOn;
        public bool attackButtonEndOff;

        public bool dialogueButtonOn;
        public bool dialogueButtonOff;

        public bool dialogueButtonEndOn;
        public bool dialogueButtonEndOff;

        public bool inventoryButtonOn;
        public bool inventoryButtonOff;

        public bool inventoryButtonEndOn;
        public bool inventoryButtonEndOff;

        public bool stealthButtonOn;
        public bool stealthButtonOff;

        public bool stealthButtonEndOn;
        public bool stealthButtonEndOff;

        public bool useButtonOn;
        public bool useButtonOff;

        public bool useButtonEndOn;
        public bool useButtonEndOff;

        public Item[] items;

        public bool checkGrenade_checkHeal;
        public bool checkEnemyHealth;
        public bool checkUseHeal;

        public bool dragAndDropOn;
        public bool dragAndDropOff;

        public bool dragAndDropEndOn;
        public bool dragAndDropEndOff;

        public bool stealTrainingOn;
        public bool stealTrainingOff;

        public bool craftButtonEndOn;
        public bool craftButtonEndOff;

        public bool statsButtonEndOn;
        public bool statsButtonEndOff;

        public bool questButtonEndOn;
        public bool questButtonEndOff;

        public bool combatTrainingEndOn;
        public bool combatTrainingEndOff;

        public bool grenadeButtonEndOn;
        public bool grenadeButtonOff;

        public bool healButtonEndOn;
        public bool healButtonOff;

        public bool Complete;
        public bool Active;

        public int NextPart;
    }
}