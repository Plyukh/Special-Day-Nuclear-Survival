using UnityEngine;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] LanguageManager languageManager;

    [SerializeField] private Character player;
    [SerializeField] private Image barterIcon;

    public Slot dragSlot;

    [SerializeField] private Slot currentSlot1;
    [SerializeField] private Slot currentSlot2;

    [SerializeField] private Animator[] protection;
    [SerializeField] private Animator[] weaponInfo;
    [SerializeField] private RectTransform infoObject;

    private GameObject stealIconObject;
    private GameObject barterIconObject;

    private int stealChance;
    private int currentCost;

    [HideInInspector] public bool training;
    [HideInInspector] public bool trainingSteal;

    public void OnBeginDrag(Slot slot)
    {
        if (training)
        {
            return;
        }

        if (slot.item != null)
        {
            PlayAudio(2);

            currentSlot1 = slot;

            dragSlot.item = slot.item;
            dragSlot.slotType = slot.slotType;
            dragSlot.inventory = slot.inventory;
            dragSlot.ActiveSlot(false);

            currentSlot1.ActiveSlot(true);

            if (player.Inventory.barter)
            {
                Image BarterIcon = Instantiate(barterIcon, new Vector2(dragSlot.transform.position.x + 50, dragSlot.transform.position.y + 125), dragSlot.transform.rotation, dragSlot.transform);
                Text BarterText = BarterIcon.transform.GetChild(0).GetChild(0).GetComponent<Text>();

                currentCost = dragSlot.item.cost + (dragSlot.item.cost / 100 * dragSlot.inventory.transform.parent.GetComponent<Character>().FindSkill(Skills.Barter).points) -
                               (dragSlot.item.cost / 100 * player.FindSkill(Skills.Barter).points);

                if (dragSlot.item.stack)
                {
                    currentCost *= (int)dragSlot.item.number;
                }

                float Discount = 0;
                float newCost = 0;

                //Barter Perk
                if (player.PerkSystem.FindPerk(Skills.Barter, 0).Active)
                {
                    newCost = currentCost;
                    Discount = newCost / 100 * 20;
                    newCost = currentCost - Discount;
                    if(newCost <= 0)
                    {
                        newCost = 1;
                    }
                    currentCost = Mathf.RoundToInt(newCost);
                }

                BarterText.text = currentCost.ToString();
                barterIconObject = BarterIcon.gameObject;
            }
            else if (dragSlot.inventory.transform.parent.tag == "NPC")
            {
                if (dragSlot.inventory.gameObject.transform.parent.GetComponent<HealthSystem>().health > 0)
                {
                    if (player.stealthSystem.stealth)
                    {
                        Image stealIcon = Instantiate(player.stealthSystem.stealIcon, new Vector2(dragSlot.transform.position.x + 50, dragSlot.transform.position.y + 125), dragSlot.transform.rotation, dragSlot.transform);
                        Text stealChanceText = stealIcon.transform.GetChild(0).GetChild(0).GetComponent<Text>();

                        stealChance = player.FindSkill(Skills.Steal).points - dragSlot.item.weight;

                        //Steal Perk
                        if (player.PerkSystem.FindPerk(Skills.Steal, 0).Active)
                        {
                            stealChance += 10;
                        }

                        if (player.characterMovement.CurrentRoom.GetComponent<Room>().light)
                        {
                            stealChance -= 25;
                        }
                        else
                        {
                            stealChance += 25;
                        }

                        if (dragSlot.item.equipped)
                        {
                            stealChance = 0;
                        }

                        if (stealChance < 0)
                        {
                            stealChance = 0;
                        }
                        else if (stealChance > 100 || trainingSteal)
                        {
                            stealChance = 100;
                        }

                        stealChanceText.text = stealChance.ToString() + "%";
                        stealChanceText.color = SelectColor(stealChance);

                        stealIconObject = stealIcon.gameObject;
                    }
                }
            }
        }
    }

    public void OnDrag()
    {
        if(dragSlot != null)
        {
            dragSlot.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag()
    {
        if(currentSlot2 != null)
        {
            if ((currentSlot1.slotType == currentSlot2.slotType) ||
                 (currentSlot1.item.itemType == currentSlot2.slotType) ||
                 (currentSlot2.item != null && currentSlot1.item.itemType == currentSlot2.item.itemType) ||
                 (currentSlot1.slotType != ItemType.Other && currentSlot2.slotType == ItemType.Other && currentSlot2.item == null))
            {
                int stealRandom = -1;
                int currentCost = -1;
                if (player.stealthSystem.stealth && (currentSlot1.inventory.transform.parent.tag == "NPC" || currentSlot2.inventory.transform.parent.tag == "NPC") &&
                    currentSlot1.inventory != currentSlot2.inventory)
                {
                    if (currentSlot1.inventory.transform.parent.tag == "Player")
                    {
                        stealRandom = 101;
                    }
                    else if(currentSlot1.inventory.transform.parent.GetComponent<HealthSystem>().health > 0)
                    {
                        stealRandom = Random.Range(0, 101);
                    }
                }
                else if (player.Inventory.barter && (currentSlot1.inventory.transform.parent.tag == "NPC" || currentSlot2.inventory.transform.parent.tag == "NPC") && 
                         currentSlot1.inventory != currentSlot2.inventory)
                {
                    currentCost = this.currentCost;
                }

                if (stealRandom <= stealChance)
                {
                    if(currentCost > -1)
                    {
                        if (currentSlot2.inventory.CanAdd(dragSlot.item) && currentSlot2.inventory.money >= currentCost)
                        {
                            player.Inventory.AddMoney(currentSlot2.inventory, currentSlot1.inventory, currentCost);
                            dragSlot.item.sell = true;

                            currentSlot2.inventory.AddItem(dragSlot.item);

                            currentSlot1.inventory.items[currentSlot1.id] = null;
                            currentSlot1.item = null;
                            currentSlot1.UpdateStot();
                            currentSlot2.inventory.ShowInventory(true);
                        }
                        else
                        {
                            currentSlot1.ActiveSlot(false);
                        }

                        PlayAudio(1);

                        dragSlot.ActiveSlot(true);
                        currentSlot1 = null;
                        Destroy(barterIconObject);

                        return;
                    }

                    if (dragSlot.item.itemType == ItemType.Backpack && currentSlot2.slotType == ItemType.Backpack)
                    {
                        BackpackItem backpackItem = dragSlot.item.GetComponent<BackpackItem>();
                        int x = 0;
                        for (int i = 0; i < player.Inventory.slots.Length; i++)
                        {
                            if (player.Inventory.slots[i].slotType == ItemType.Other && player.Inventory.slots[i].active)
                            {
                                x += 1;
                            }
                        }
                        if (backpackItem.dopSlots >= x)
                        {
                            currentSlot1.inventory.AddItem(currentSlot1, currentSlot2.item);
                            currentSlot2.inventory.AddItem(currentSlot2, dragSlot.item);
                            currentSlot1.UpdateStot();
                            currentSlot2.UpdateStot();
                        }
                        else
                        {
                            currentSlot1.ActiveSlot(false);
                        }
                    }
                    else if (currentSlot1.slotType == ItemType.Backpack && currentSlot2.slotType == ItemType.Other)
                    {
                        currentSlot1.ActiveSlot(false);
                    }
                    else if (dragSlot.item.stack && currentSlot2.inventory.FindItem(dragSlot.item) && currentSlot1.inventory != currentSlot2.inventory)
                    {
                        for (int i = 0; i < currentSlot2.inventory.slots.Length; i++)
                        {
                            if (currentSlot2.inventory.slots[i].item != null)
                            {
                                if (currentSlot2.inventory.slots[i].item.itemName == dragSlot.item.itemName)
                                {
                                    currentSlot2.inventory.slots[i].item.number += dragSlot.item.number;
                                    currentSlot2.inventory.slots[i].UpdateStot();
                                    Destroy(currentSlot1.item);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        currentSlot1.inventory.AddItem(currentSlot1, currentSlot2.item);
                        currentSlot2.inventory.AddItem(currentSlot2, dragSlot.item);
                        currentSlot1.UpdateStot();
                        currentSlot2.UpdateStot();
                    }
                }
                else
                {
                    if (currentSlot1.inventory.transform.parent.tag == "Player" && currentSlot2.inventory.transform.parent.tag == "NPC")
                    {
                        currentSlot1.ActiveSlot(false);
                    }
                    else
                    {
                        Camera.main.GetComponent<CameraZoom>().OnExitObject();
                        currentSlot1.inventory.CloseInventory();
                        player.characterMovement.MoveToCombat();
                        player.stealthSystem.BotchedSteal();

                        currentSlot1.ActiveSlot(false);
                    }
                }
            }
            else
            {
                currentSlot1.ActiveSlot(false);
            }
        }
        else
        {
            currentSlot1.ActiveSlot(false);
        }

        dragSlot.ActiveSlot(true);
        PlayAudio(1);

        currentSlot1 = null;

        Destroy(stealIconObject);
        Destroy(barterIconObject);

        player.Inventory.CheckCarryWeight();
    }

    public void OnPointerEnter(Slot eventData)
    {
        currentSlot2 = eventData;
        if(currentSlot2.item != null && currentSlot1 == null)
        {
            if (languageManager.currentLanguage == Language.Russian)
            {
                ShowInfo(true, currentSlot2.item.itemName, currentSlot2.item.itemDescription);
            }
            else if (languageManager.currentLanguage == Language.English)
            {
                ShowInfo(true, currentSlot2.item.englishItemName, currentSlot2.item.englishItemDescription);
            }
            else if (languageManager.currentLanguage == Language.Indonesian)
            {
                ShowInfo(true, currentSlot2.item.indonesianItemName, currentSlot2.item.indonesianItemDescription);
            }
        }
    }

    public void OnPointerExit()
    {
        currentSlot2 = null;
        ShowInfo(false);
    }

    private void PlayAudio(float pitch)
    {
        audioSource.pitch = pitch;
        audioSource.Play();
    }

    public void NewStats(Slot slot, ItemType itemType)
    {
        if(itemType == ItemType.Weapon)
        {
            player.SetWeaponTrigger();
            for (int i = 0; i < weaponInfo.Length; i++)
            {
                weaponInfo[i].SetTrigger("New Stats");
            }
        }
        else if(itemType == ItemType.Armor)
        {
            player.UpdateArmor();
            for (int i = 0; i < protection.Length; i++)
            {
                if (protection[i].gameObject.activeInHierarchy)
                {
                    protection[i].SetTrigger("New Stats");
                }
            }
        }

        UpdateStats(slot);
    }

    public void UpdateStats(Slot slot)
    {
        if (slot.slotType == ItemType.Armor)
        {
            for (int i = 0; i < protection.Length; i++)
            {
                HealthSystem healthSystem = player.GetComponent<HealthSystem>();

                Text text = protection[i].transform.GetChild(1).GetComponent<Text>();
                int value = 0;
                if (slot.item != null)
                {
                    value = slot.item.GetComponent<ArmorItem>().protection[i] + healthSystem.protection[i];
                }
                else
                {
                    value = healthSystem.protection[i];
                }

                text.text = value + "%";
                text.color = SelectColor(value);
            }
        }
        else if (slot.slotType == ItemType.Weapon)
        {
            //Штраф по Силе
            int StrengthPenalty = 0;

            if(player.currentWeapon != null)
            {
                StrengthPenalty = player.currentWeapon.needStrength - player.Attributes.Strength;
            }

            //MeleeWeapons Perk
            if (player.PerkSystem.FindPerk(Skills.MeleeWeapons, 1).Active)
            {
                StrengthPenalty -= 1;
            }

            if (StrengthPenalty < 0)
            {
                StrengthPenalty = 0;
            }

            //Вероятность Попадания
            float HitChance = 0;

            if (slot.item != null)
            {
                WeaponItem weapon = slot.item.GetComponent<WeaponItem>();

                float Distance = weapon.distance;

                if (weapon.weaponType == WeaponType.OneHandMeleeWeapon || weapon.weaponType == WeaponType.TwoHandsMeleeWeapon)
                {
                    //Вероятность Попадания
                    HitChance = 25 + Mathf.CeilToInt(player.FindSkill(Skills.MeleeWeapons).points / 2) - (20 * StrengthPenalty);

                    //Количество патриков
                    UpdateAmmoText(null);
                }
                else
                {
                    //Штраф по Силе
                    if (weapon.needStrength > player.Attributes.Strength)
                    {
                        StrengthPenalty = weapon.needStrength - player.Attributes.Strength;
                    }

                    //Вероятность Попадания
                    if (weapon.englishItemName == "Flamethrower" || weapon.weaponType == WeaponType.RocketLauncher)
                    {
                        HitChance = 100 - (20 * StrengthPenalty);
                    }
                    else
                    {
                        HitChance = 25 + Mathf.CeilToInt(player.FindSkill(Skills.Guns).points / 2) - (20 * StrengthPenalty);
                    }

                    //Guns Perk
                    if (player.PerkSystem.FindPerk(Skills.Guns, 0).Active)
                    {
                        HitChance += 5;
                    }
                    if (player.PerkSystem.FindPerk(Skills.Guns, 1).Active)
                    {
                        HitChance += 10;
                        Distance *= 2f;
                    }

                    //Количество патриков
                    UpdateAmmoText(weapon);
                }

                // Дамаге
                weaponInfo[0].transform.GetChild(1).GetComponent<Text>().text = weapon.minDamage + "-" + weapon.maxDamage;
                for (int i = 0; i < weaponInfo[0].transform.GetChild(0).childCount; i++)
                {
                    if (weaponInfo[0].transform.GetChild(0).GetChild(i).gameObject.name == weapon.damageType.ToString())
                    {
                        weaponInfo[0].transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
                    }
                    else
                    {
                        weaponInfo[0].transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
                    }
                }
                // Дистанция
                weaponInfo[2].transform.GetChild(1).GetComponent<Text>().text = Distance.ToString();

                //Требование к Силе

                   //MeleeWeapons Perk
                if (player.PerkSystem.FindPerk(Skills.MeleeWeapons, 1).Active)
                {
                    weaponInfo[4].transform.GetChild(1).GetComponent<Text>().text = (weapon.needStrength - 1).ToString();

                    if (weapon.needStrength - 1 > player.Attributes.Strength)
                    {
                        weaponInfo[4].transform.GetChild(1).GetComponent<Text>().color = Color.red;
                        EffectsUI.NotEnoughPowerEffect(true);
                    }
                    else
                    {
                        weaponInfo[4].transform.GetChild(1).GetComponent<Text>().color = Color.green;
                        EffectsUI.NotEnoughPowerEffect(false);
                    }
                }
                else
                {
                    weaponInfo[4].transform.GetChild(1).GetComponent<Text>().text = weapon.needStrength.ToString();

                    if (weapon.needStrength > player.Attributes.Strength)
                    {
                        weaponInfo[4].transform.GetChild(1).GetComponent<Text>().color = Color.red;
                        EffectsUI.NotEnoughPowerEffect(true);
                    }
                    else
                    {
                        weaponInfo[4].transform.GetChild(1).GetComponent<Text>().color = Color.green;
                        EffectsUI.NotEnoughPowerEffect(false);
                    }
                }
            }
            else
            {
                //Вероятность Попадания
                HitChance = 25 + Mathf.CeilToInt(player.FindSkill(Skills.Unarmed).points / 2) - (20 * StrengthPenalty);

                //Дамаге
                int minDamage = Mathf.CeilToInt(player.FindSkill(Skills.Unarmed).points / 25);
                int maxDamage = Mathf.CeilToInt(player.FindSkill(Skills.Unarmed).points / 25 + (player.Attributes.Strength + player.Attributes.Agility));

                //Unarmed Perk
                if (player.PerkSystem.FindPerk(Skills.Unarmed, 0).Active)
                {
                    maxDamage += 5;
                }
                if (player.PerkSystem.FindPerk(Skills.Unarmed, 2).Active)
                {
                    minDamage += 5;
                }

                weaponInfo[0].transform.GetChild(1).GetComponent<Text>().text = minDamage + "-" + maxDamage;
                for (int i = 0; i < weaponInfo[0].transform.GetChild(0).childCount; i++)
                {
                    if (i == 0)
                    {
                        weaponInfo[0].transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
                    }
                    else
                    {
                        weaponInfo[0].transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
                    }
                }
                //Дистанция
                weaponInfo[2].transform.GetChild(1).GetComponent<Text>().text = "1,75";

                //Количество патриков
                UpdateAmmoText(null);

                //Требование к Силе
                weaponInfo[4].transform.GetChild(1).GetComponent<Text>().text = player.Attributes.Strength.ToString();
                weaponInfo[4].transform.GetChild(1).GetComponent<Text>().color = Color.white;
                EffectsUI.NotEnoughPowerEffect(false);
            }

            //Steal Perk
            if (player.PerkSystem.FindPerk(Skills.Steal, 1).Active)
            {
                if (player.stealthSystem.light == false || player.PerkSystem.FindPerk(Skills.Steal, 2).Active)
                {
                    HitChance += 15;
                }
            }

            //Вероятность Попадания
            if (HitChance > 100)
            {
                HitChance = 100;
            }
            else if (HitChance < 0)
            {
                HitChance = 0;
            }
            weaponInfo[1].transform.GetChild(1).GetComponent<Text>().text = HitChance.ToString() + "%";
        }
    }

    public void UpdateAmmoText(WeaponItem weapon)
    {
        for (int i = 0; i < weaponInfo[3].transform.GetChild(0).childCount; i++)
        {
            weaponInfo[3].transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
            if(weapon != null)
            {
                if(weapon.ammoName == weaponInfo[3].transform.GetChild(0).GetChild(i).gameObject.name)
                {
                    weaponInfo[3].transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
                }
            }
        }

        if(weapon != null)
        {
            float bulletValue = 0;
            for (int i = 0; i < player.Inventory.items.Length; i++)
            {
                if(player.Inventory.items[i] != null)
                {
                    if (player.Inventory.items[i].itemType == ItemType.Ammo)
                    {
                        if (player.Inventory.items[i].itemName == weapon.ammoName)
                        {
                            bulletValue += player.Inventory.items[i].number;
                        }
                    }
                }
            }
            weaponInfo[3].transform.GetChild(1).GetComponent<Text>().text = bulletValue.ToString();
        }
        else
        {
            weaponInfo[3].transform.GetChild(1).GetComponent<Text>().text = "";
        }
    }

    void ShowInfo(bool Value,string Name = null, string Description = null)
    {
        infoObject.gameObject.SetActive(Value);
        infoObject.transform.GetChild(1).GetComponent<Text>().text = Name;
        infoObject.transform.GetChild(2).GetComponent<Text>().text = "";
        if (Value)
        {
            infoObject.anchoredPosition = currentSlot2.infoPosition;

            infoObject.transform.GetChild(5).GetComponent<Text>().text = currentSlot2.item.cost.ToString();
            infoObject.transform.GetChild(6).GetComponent<Text>().text = currentSlot2.item.weight.ToString();

            if (currentSlot2.item.itemType == ItemType.Medkit)
            {
                MedkitItem medkit = currentSlot2.item.GetComponent<MedkitItem>();
                float x = medkit.maxHeal - medkit.minHeal;
                int newMin = medkit.minHeal + Mathf.RoundToInt(x / 100 * player.FindSkill(Skills.Doctor).points);

                if(medkit.maxHeal > 0)
                {
                    if (newMin == medkit.maxHeal)
                    {
                        if(languageManager.currentLanguage == Language.Russian)
                        {
                            infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Лечение: " + medkit.maxHeal + " ]" + '\n' + Description;
                        }
                        else if(languageManager.currentLanguage == Language.English)
                        {
                            infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Healing: " + medkit.maxHeal + " ]" + '\n' + Description;
                        }
                        else if (languageManager.currentLanguage == Language.Indonesian)
                        {
                            infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Penyembuhan: " + medkit.maxHeal + " ]" + '\n' + Description;
                        }
                    }
                    else
                    {
                        if (languageManager.currentLanguage == Language.Russian)
                        {
                            infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Лечение: " + newMin + "-" + medkit.maxHeal + " ]" + '\n' + Description;
                        }
                        else if (languageManager.currentLanguage == Language.English)
                        {
                            infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Healing: " + newMin + "-" + medkit.maxHeal + " ]" + '\n' + Description;
                        }
                        else if (languageManager.currentLanguage == Language.Indonesian)
                        {
                            infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Penyembuhan: " + newMin + "-" + medkit.maxHeal + " ]" + '\n' + Description;
                        }
                    }
                }
                else if(medkit.removeRadiation > 0)
                {
                    if (languageManager.currentLanguage == Language.Russian)
                    {
                        infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Выводит радиацию: " + medkit.removeRadiation + " ]" + '\n' + Description;
                    }
                    else if (languageManager.currentLanguage == Language.English)
                    {
                        infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Removes radiation: " + medkit.removeRadiation + " ]" + '\n' + Description;
                    }
                    else if (languageManager.currentLanguage == Language.Indonesian)
                    {
                        infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Menghilangkan radiasi: " + medkit.removeRadiation + " ]" + '\n' + Description;
                    }
                }
            }
            else if (currentSlot2.item.itemType == ItemType.Weapon || currentSlot2.item.itemType == ItemType.Grenade)
            {
                WeaponItem weapon = currentSlot2.item.GetComponent<WeaponItem>();

                if (languageManager.currentLanguage == Language.Russian)
                {
                    infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Урон: " + weapon.minDamage + "-" + weapon.maxDamage + " ]" + '\n' + Description;
                }
                else if (languageManager.currentLanguage == Language.English)
                {
                    infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Damage: " + weapon.minDamage + "-" + weapon.maxDamage + " ]" + '\n' + Description;
                }
                else if (languageManager.currentLanguage == Language.Indonesian)
                {
                    infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Kerusakan: " + weapon.minDamage + "-" + weapon.maxDamage + " ]" + '\n' + Description;
                }
            }
            else if (currentSlot2.item.itemType == ItemType.Backpack)
            {
                BackpackItem backpack = currentSlot2.item.GetComponent<BackpackItem>();

                if (languageManager.currentLanguage == Language.Russian)
                {
                    infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Дополнительные ячейки: " + (backpack.dopSlots - 10) + " ]" + '\n' + Description;
                }
                else if (languageManager.currentLanguage == Language.English)
                {
                    infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Additional cells: " + (backpack.dopSlots - 10) + " ]" + '\n' + Description;
                }
                else if (languageManager.currentLanguage == Language.Indonesian)
                {
                    infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Sel tambahan: " + (backpack.dopSlots - 10) + " ]" + '\n' + Description;
                }
            }
            else if (currentSlot2.item.itemType == ItemType.Other)
            {
                BackpackItem backpack = currentSlot2.item.GetComponent<BackpackItem>();

                if (languageManager.currentLanguage == Language.Russian)
                {
                    infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Предмет Задания ]" + '\n' + Description;
                }
                else if (languageManager.currentLanguage == Language.English)
                {
                    infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ The Subject of the Assignment ]" + '\n' + Description;
                }
                else if (languageManager.currentLanguage == Language.Indonesian)
                {
                    infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Subjek Tugas ]" + '\n' + Description;
                }
            }
            else if (currentSlot2.item.itemType == ItemType.Craft)
            {
                if (languageManager.currentLanguage == Language.Russian)
                {
                    infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Компонент ]" + '\n' + Description;
                }
                else if (languageManager.currentLanguage == Language.English)
                {
                    infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Component ]" + '\n' + Description;
                }
                else if (languageManager.currentLanguage == Language.Indonesian)
                {
                    infoObject.transform.GetChild(2).GetComponent<Text>().text = "[ Komponen ]" + '\n' + Description;
                }
            }
            else
            {
                infoObject.transform.GetChild(2).GetComponent<Text>().text = Description;
            }
        }
    }

    private Color32 SelectColor(int value)
    {
        if (value < 26)
        {
            return Color.red;
        }
        else if (value > 25 && value < 51)
        {
            return new Color32(255, 165, 0, 255);
        }
        else if (value > 50 && value < 76)
        {
            return Color.yellow;
        }
        else if (value > 75)
        {
            return Color.green;
        }
        else
        {
            return Color.white;
        }
    }
}