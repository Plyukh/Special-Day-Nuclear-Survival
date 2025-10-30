using UnityEngine;
using UnityEngine.UI;

public class CraftSystem : MonoBehaviour
{
    [SerializeField] private Character player;
    [SerializeField] private LanguageManager languageManager;

    [SerializeField] private Image itemImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text numberText;
    [SerializeField] private Text skillText;
    [SerializeField] private Button craftButton;

    [SerializeField] private Slot[] craftSlots;

    private Blueprint currentBlueprint;

    public void ShowBlueprint(Blueprint blueprint)
    {
        currentBlueprint = blueprint;

        craftButton.interactable = true;
        craftButton.transform.GetChild(1).GetComponent<Text>().color = Color.white;

        itemImage.sprite = blueprint.blueprintItem.itemSprite;

        if(languageManager.currentLanguage == Language.Russian)
        {
            nameText.text = blueprint.blueprintItem.itemName;
        }
        else if (languageManager.currentLanguage == Language.English)
        {
            nameText.text = blueprint.blueprintItem.englishItemName;
        }
        else if (languageManager.currentLanguage == Language.Indonesian)
        {
            nameText.text = blueprint.blueprintItem.indonesianItemName;
        }

        int points = player.FindSkill(Skills.Science).points;

        skillText.text = points + "/" + blueprint.needScienceSkill;

        if (blueprint.blueprintItem.stack)
        {
            if (currentBlueprint.blueprintItem.itemType == ItemType.Ammo)
            {
                if (player.PerkSystem.FindPerk(Skills.Science, 0).Active)
                {
                    for (int i = 0; i < currentBlueprint.transform.parent.childCount; i++)
                    {
                        currentBlueprint.transform.parent.GetChild(i).GetComponent<Blueprint>().UpdateNumberText();
                    }

                    int newAmmo = Mathf.RoundToInt(currentBlueprint.blueprintItem.number / 4 + currentBlueprint.blueprintItem.number);
                    numberText.text = newAmmo.ToString();
                }
                else
                {
                    numberText.text = blueprint.blueprintItem.number.ToString();
                }
            }
            else
            {
                numberText.text = blueprint.blueprintItem.number.ToString();
            }
        }
        else
        {
            numberText.text = "";
        }

        if(points >= blueprint.needScienceSkill)
        {
            skillText.color = Color.green;
        }
        else
        {
            skillText.color = Color.red;
            craftButton.interactable = false;
            craftButton.transform.GetChild(1).GetComponent<Text>().color = Color.gray;
        }

        for (int i = 0; i < craftSlots.Length; i++)
        {
            if(i < blueprint.needItems.Length)
            {
                craftSlots[i].gameObject.SetActive(true);
                craftSlots[i].item = blueprint.needItems[i];
                craftSlots[i].UpdateStot();

                if(player.Inventory.FindItem(blueprint.needItems[i]) != null)
                {
                    craftSlots[i].SelectSpriteColor(Color.green);
                }
                else
                {
                    craftSlots[i].SelectSpriteColor(Color.red);
                    craftButton.interactable = false;
                    craftButton.transform.GetChild(1).GetComponent<Text>().color = Color.gray;
                }
            }
            else
            {
                craftSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void CraftItem()
    {
        Item item = null;
        if (currentBlueprint.blueprintItem.stack)
        {
            item = Instantiate(currentBlueprint.blueprintItem, currentBlueprint.transform);

            if (currentBlueprint.blueprintItem.itemType == ItemType.Ammo)
            {
                if (player.PerkSystem.FindPerk(Skills.Science, 0).Active)
                {
                    int newAmmo = Mathf.RoundToInt(currentBlueprint.blueprintItem.number / 4 + currentBlueprint.blueprintItem.number);
                    item.number = newAmmo;
                }
            }
            if (currentBlueprint.blueprintItem.englishItemName == "Pills")
            {
                if (player.PerkSystem.FindPerk(Skills.Science, 1).Active)
                {
                    int random = Random.Range(0, 4);
                    if (random == 3)
                    {
                        item.number = item.number + 1;
                    }
                }
            }
        }

        for (int i = 0; i < currentBlueprint.needItems.Length; i++)
        {
            player.Inventory.AddItem(player.Inventory.FindSlot(currentBlueprint.needItems[i]), null);
            if (i == 0)
            {
                if (currentBlueprint.blueprintItem.stack)
                {
                    player.Inventory.AddItem(item);
                }
                else
                {
                    player.Inventory.AddItem(currentBlueprint.blueprintItem);
                }
            }
        }

        if (currentBlueprint.blueprintItem.englishItemName == "Flamethrower")
        {
            //googlePlayAchievements.UnlockAchievement(15);
        }

        ShowBlueprint(currentBlueprint);

        player.GetComponent<GrenadeUIManager>().UpdateButton();
        player.GetComponent<HealthSystem>().UpdateButton();

        GetComponent<Animator>().SetTrigger("Craft");
        ExperienceSystem.AddXP(25);
    }
}