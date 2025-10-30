using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Character character;

    private GameObject mainPanel;
    private GameObject panel;
    private GameObject characterPanel;

    public Item[] items;

    public Slot[] slots;

    public Slot armorSlot;
    public Slot weaponSlot;
    public Slot grenadeSlot;
    public Slot medicineSlot;
    public Slot backpackSlot;

    private Text carryWeightText;
    private Slider carryWeightSlider;
    public float currentCarryWeight;
    public float maxCarryWeight;

    public int money;

    private Text moneyText;
    public GameObject takeAllButton;

    public AudioClip[] moneyClips;
    public Door[] doors;
    public Container[] containers;

    [SerializeField] GameObject LockPickingIcon;

    public bool barter;

    public void StartInventory()
    {
        mainPanel = GameObject.Find("Canvas").transform.GetChild(11).gameObject;

        if(transform.parent.tag != "Player")
        {
            if (FindFirstObjectByType<SceneManagerScript>().reset == true)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    items[i] = transform.GetChild(i).GetComponent<Item>();
                }
            }
        }

        if (character != null && character.tag == "NPC")
        {
            if(character.characterMovement.Target == null)
            {
                character.Inventory.TakeEquppedWeapon();
            }
            character.Inventory.TakeEquppedArmor();
        }

        if (transform.parent.tag == "Player")
        {
            panel = mainPanel.transform.GetChild(0).gameObject;
            characterPanel = mainPanel.transform.GetChild(1).gameObject;
            moneyText = panel.transform.GetChild(2).GetChild(1).GetComponent<Text>();
            carryWeightText = panel.transform.GetChild(3).GetChild(1).GetComponent<Text>();
            carryWeightSlider = panel.transform.GetChild(3).GetChild(2).GetChild(0).GetChild(0).GetComponent<Slider>();
            for (int i = 0; i < items.Length; i++)
            {
                slots[i].item = items[i];
                slots[i].inventory = this;
                slots[i].id = i;
                if(slots[i].slotType != ItemType.Weapon)
                {
                    slots[i].UpdateStot();
                }
                else if(slots[i].item != null)
                {
                    slots[i].UpdateStot();
                }
            }
            CheckCarryWeight();
        }
        else if (transform.parent.tag != "Player")
        {
            panel = mainPanel.transform.GetChild(2).gameObject;
            for (int i = 0; i < panel.transform.GetChild(0).childCount; i++)
            {
                slots[i] = panel.transform.GetChild(0).GetChild(i).GetComponent<Slot>();
                slots[i].id = i;
            }
            moneyText = panel.transform.GetChild(1).GetChild(1).GetComponent<Text>();
        }
    }

    public void CloseInventoryButton()
    {
        if (barter == false)
        {
            Camera.main.GetComponent<CameraZoom>().OnExitObject();
        }
        Camera.main.GetComponent<CameraZoom>().InventoryCamera(false);
    }

    public void ShowInventory(bool barter = false)
    {
        Camera.main.GetComponent<CameraZoom>().OnPointerObject();
        Camera.main.GetComponent<CameraZoom>().InventoryCamera(true);

        mainPanel.SetActive(true);
        panel.SetActive(true);

        this.barter = barter;

        if (transform.parent.tag != "Player")
        {
            moneyText.transform.parent.gameObject.SetActive(barter);
            for (int i = 0; i < panel.transform.GetChild(0).childCount; i++)
            {
                slots[i] = panel.transform.GetChild(0).GetChild(i).GetComponent<Slot>();
                if (i > items.Length - 1)
                {
                    slots[i].active = false;
                    slots[i].item = null;
                }
                else
                {
                    if(barter)
                    {
                        if (items[i] != null && items[i].sell)
                        {
                            slots[i].item = items[i];
                        }
                    }
                    else
                    {
                        slots[i].item = items[i];
                    }
                    slots[i].inventory = this;
                    slots[i].active = true;
                }

                slots[i].UpdateStot();
            }

            bool findKey = false;

            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i].needSkill != null)
                {
                    if ((character != null && character.GetComponent<HealthSystem>().health <= 0) || character == null)
                    {
                        doors[i].OpenDoor();
                        findKey = true;
                    }
                }
            }
            for (int i = 0; i < containers.Length; i++)
            {
                if (containers[i].needSkill != null)
                {
                    if ((character != null && character.GetComponent<HealthSystem>().health <= 0) || character == null)
                    {
                        containers[i].OpenContainer();
                        findKey = true;
                    }
                }
            }

            if (barter == false && findKey)
            {
                LockPickingIcon.SetActive(false);
                LockPickingIcon.SetActive(true);
            }
        }
        else
        {
            CheckCarryWeight();

            if (barter || character.stealthSystem.stealth)
            {
                takeAllButton.gameObject.SetActive(false);
            }
            else
            {
                takeAllButton.gameObject.SetActive(true);
            }

            for (int i = 0; i < items.Length; i++)
            {
                slots[i].item = items[i];
                slots[i].inventory = this;
                if(slots[i].slotType != ItemType.Weapon)
                {
                    slots[i].UpdateStot();
                }
                else
                {
                    slots[i].UpdateStot(false);
                }
            }
        }
        //if (weaponSlot != null)
        //{
        //    weaponSlot.UpdateStot();
        //}
        CheckMoney();
    }
    public void ShowCharacterSlots()
    {
        characterPanel.SetActive(true);
    }

    public void AddItem(Item item)
    {
        if (item.stack && FindSlot(item))
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].active)
                {
                    if (slots[i].item != null)
                    {
                        if (item.stack && item.itemName == slots[i].item.itemName)
                        {
                            slots[i].item.number += item.number;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].active && slots[i].slotType == ItemType.Other)
                {
                    if (slots[i].item == null)
                    {
                        items[i] = item;
                        slots[i].item = item;
                        break;
                    }
                }
            }
        }
    }
    public void AddItem(Slot slot ,Item item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].name == slot.name)
            {
                items[i] = item;
                slot.item = item;
                break;
            }
        }
    }
    public bool CanAdd(Item item)
    {
        if (item.stack && FindSlot(item))
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].active)
                {
                    if (slots[i].item != null)
                    {
                        if (item.stack && item.itemName == slots[i].item.itemName)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].active && slots[i].slotType == ItemType.Other)
                {
                    if (slots[i].item == null)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public void TakeAll()
    {
        Inventory lootInventory = GameObject.Find("Loot").transform.GetChild(0).GetChild(0).GetComponent<Slot>().inventory;

        int y = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].slotType == ItemType.Other && slots[i].active)
            {
                y += 1;
            }
        }

        for (int i = 0; i < lootInventory.items.Length; i++)
        {
            if (lootInventory.items[i] != null)
            {
                for (int j = 0; j < y; j++)
                {
                    if (lootInventory.items[i].stack == false)
                    {
                        if (slots[j].active)
                        {
                            if (slots[j].slotType == ItemType.Other && slots[j].item == null)
                            {
                                items[j] = lootInventory.items[i];
                                slots[j].item = lootInventory.items[i];
                                slots[j].UpdateStot();
                                lootInventory.items[i] = null;
                                lootInventory.slots[i].item = null;
                                lootInventory.slots[i].UpdateStot();
                                break;
                            }
                        }
                    }
                    else
                    {
                        bool findStack = false;

                        if(grenadeSlot.item != null && grenadeSlot.item.itemName == lootInventory.items[i].itemName)
                        {
                            grenadeSlot.item.number += lootInventory.items[i].number;
                            grenadeSlot.UpdateStot();
                            lootInventory.items[i] = null;
                            lootInventory.slots[i].item = null;
                            lootInventory.slots[i].UpdateStot();
                            character.GetComponent<GrenadeUIManager>().UpdateButton();
                            break;
                        }
                        else if(medicineSlot.item != null && medicineSlot.item.itemName == lootInventory.items[i].itemName)
                        {
                            medicineSlot.item.number += lootInventory.items[i].number;
                            medicineSlot.UpdateStot();
                            lootInventory.items[i] = null;
                            lootInventory.slots[i].item = null;
                            lootInventory.slots[i].UpdateStot();
                            character.GetComponent<HealthSystem>().UpdateButton();
                            break;
                        }

                        for (int k = 0; k < y; k++)
                        {
                            if (slots[k].active)
                            {
                                if (slots[k].slotType == ItemType.Other && slots[k].item != null)
                                {
                                    if (slots[k].item.itemName == lootInventory.items[i].itemName)
                                    {
                                        slots[k].item.number += lootInventory.items[i].number;
                                        slots[k].UpdateStot();
                                        lootInventory.items[i] = null;
                                        lootInventory.slots[i].item = null;
                                        lootInventory.slots[i].UpdateStot();
                                        findStack = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if(findStack == false)
                        {
                            if (slots[j].active)
                            {
                                if (slots[j].slotType == ItemType.Other && slots[j].item == null)
                                {
                                    items[j] = lootInventory.items[i];
                                    slots[j].item = lootInventory.items[i];
                                    slots[j].UpdateStot();
                                    lootInventory.items[i] = null;
                                    lootInventory.slots[i].item = null;
                                    lootInventory.slots[i].UpdateStot();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        CheckCarryWeight();
    }
    public void CloseInventory()
    {
        Camera.main.GetComponent<CameraZoom>().InventoryCamera(false);

        mainPanel.SetActive(false);
        panel.SetActive(false);

        barter = false;
    }

    public Item FindItem(Item Item)
    {
        foreach (var item in items)
        {
            if(item != null)
            {
                if (item.itemName == Item.itemName)
                {
                    return item;
                }
            }
        }

        return null;
    }
    public Slot FindSlot(Item Item)
    {
        foreach (var slot in slots)
        {
            if (slot.item != null)
            {
                if (slot.item.itemName == Item.itemName)
                {
                    return slot;
                }
            }
        }

        return null;
    }

    public void TakeEquppedWeapon()
    {
        if (character.currentWeapon == null)
        {
            foreach (var item in items)
            {
                if (item != null)
                {
                    if (item.itemType == ItemType.Weapon)
                    {
                        character.currentWeapon = item.GetComponent<WeaponItem>();
                        character.UpdateWeapon();
                        return;
                    }
                }
            }
        }
        else
        {
            foreach (var item in items)
            {
                if (item != null)
                {
                    if (item.itemType == ItemType.Weapon)
                    {
                        if (item.itemName != character.currentWeapon.itemName)
                        {
                            character.currentWeapon = item.GetComponent<WeaponItem>();
                            character.UpdateWeapon();
                            return;
                        }
                    }
                }
            }
        }
        character.currentWeapon = null;
        character.animator.SetTrigger("HideWeapon");
        character.UpdateWeapon();
    }
    public void TakeEquppedArmor()
    {
        foreach (var item in items)
        {
            if(item != null)
            {
                if (item.itemType == ItemType.Armor)
                {
                    character.currentArmor = item.GetComponent<ArmorItem>();
                    return;
                }
            }
        }
    }

    public void CheckCarryWeight()
    {
        currentCarryWeight = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if(items[i] != null)
            {
                currentCarryWeight += items[i].weight;
            }
        }

        maxCarryWeight = 25 + (character.Attributes.Strength * 25);

        carryWeightSlider.maxValue = maxCarryWeight;
        carryWeightSlider.value = currentCarryWeight;

        //Select Slider Color
        Image carryWeightSliderImage = carryWeightSlider.transform.GetChild(0).GetComponent<Image>();
        if (currentCarryWeight <= maxCarryWeight / 4)
        {
            carryWeightSliderImage.color = Color.green;
        }
        else if (currentCarryWeight > maxCarryWeight / 4 && currentCarryWeight <= maxCarryWeight / 2)
        {
            carryWeightSliderImage.color = Color.yellow;
        }
        else if (currentCarryWeight > maxCarryWeight / 2 && currentCarryWeight <= (maxCarryWeight - maxCarryWeight / 4))
        {
            carryWeightSliderImage.color = new Color32(255, 165, 0, 255);
        }
        else
        {
            carryWeightSliderImage.color = Color.red;
        }

        if (Encumbered())
        {
            carryWeightText.color = Color.red;
            if(character.tag == "Player")
            {
                LanguageManager languageManager = FindFirstObjectByType<LanguageManager>();
                if(languageManager.currentLanguage == Language.Russian)
                {
                    carryWeightText.text = "   Перегрузка!  " + currentCarryWeight + "/" + maxCarryWeight;
                }
                else if(languageManager.currentLanguage == Language.English)
                {
                    carryWeightText.text = "   Overload!  " + currentCarryWeight + "/" + maxCarryWeight;
                }
                else if (languageManager.currentLanguage == Language.Indonesian)
                {
                    carryWeightText.text = " Kelebihan beban!";
                }
            }
        }
        else
        {
            carryWeightText.color = Color.white;
            carryWeightText.text = "     " + currentCarryWeight + "/" + maxCarryWeight;
        }
    }
    public bool Encumbered()
    {
        if (currentCarryWeight <= maxCarryWeight)
        {
            EffectsUI.EncumberedEffect(false);
            return false;
        }
        else
        {
            EffectsUI.EncumberedEffect(true);
            return true;
        }
    }
    public bool ShowPanel()
    {
        if (panel.activeInHierarchy)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddMoney(Inventory WhoGives, Inventory WhoGets, int value, bool addAll = false)
    {
        if (addAll)
        {
            AddMoneyLocal(WhoGives, WhoGets, WhoGives.money);
            WhoGives.money = 0;
        }
        else if (WhoGives.money >= value && value != 0)
        {
            AddMoneyLocal(WhoGives, WhoGets, value);
        }

        void AddMoneyLocal(Inventory WhoGives, Inventory WhoGets, int value)
        {
            WhoGives.money -= value;
            WhoGets.money += value;

            AudioSource audioSourceMoney = Camera.main.GetComponent<AudioSource>();

            if (WhoGets.character.tag == "Player")
            {
                audioSourceMoney.clip = WhoGets.moneyClips[0];
            }
            else
            {
                audioSourceMoney.clip = WhoGives.moneyClips[1];
            }

            if(WhoGets.moneyText.gameObject.activeInHierarchy)
            {
                WhoGets.CheckMoney();
                WhoGets.moneyText.transform.parent.GetComponent<Animator>().SetTrigger("Money");
            }
            if (WhoGives.moneyText.gameObject.activeInHierarchy)
            {
                WhoGives.CheckMoney();
                WhoGives.moneyText.transform.parent.GetComponent<Animator>().SetTrigger("Money");
            }

            audioSourceMoney.Play();
        }
    }
    public void CheckMoney()
    {
        if (moneyText.gameObject.activeInHierarchy)
        {
            moneyText.text = money.ToString();
        }

        if(character != null)
        {
            if (character.tag == "Player")
            {
                if(money >= 10000)
                {
                    //character.googlePlayAchievements.UnlockAchievement(18);
                }
            }
        }
    }
}