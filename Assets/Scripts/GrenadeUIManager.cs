using UnityEngine;
using UnityEngine.UI;

public class GrenadeUIManager : MonoBehaviour
{
    public GameObject spawnPoint;

    [SerializeField] private Button grenadeButton;
    [SerializeField] private Sprite grenadeSprite;

    [SerializeField] private Character character;

    private Vector2 baseSize = new Vector2(65,65);
    private Vector2 newSize = new Vector2(80, 80);

    public Button GrenadeButton
    {
        get
        {
            return grenadeButton;
        }
        set
        {
            grenadeButton = value;
        }
    }

    public void UpdateButton()
    {
        if (character.Inventory.grenadeSlot.item != null)
        {
            if (character.combatSystem.combat)
            {
                grenadeButton.interactable = true;
            }
            else
            {
                grenadeButton.interactable = false;
            }

            if (character.Inventory.grenadeSlot.item.itemSprite != null)
            {
                grenadeButton.transform.GetChild(1).GetComponent<Image>().sprite = character.Inventory.grenadeSlot.item.itemSprite;
            }
            else
            {
                for (int i = 0; i < character.Inventory.grenadeSlot.ItemPrefabs.transform.childCount; i++)
                {
                    Item item = character.Inventory.grenadeSlot.ItemPrefabs.transform.GetChild(i).GetComponent<Item>();
                    if (item.itemName == character.Inventory.grenadeSlot.item.itemName)
                    {
                        grenadeButton.transform.GetChild(1).GetComponent<Image>().sprite = item.itemSprite;
                        break;
                    }
                }
            }
            grenadeButton.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = newSize;
            grenadeButton.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = character.Inventory.grenadeSlot.item.number.ToString();
        }
        else
        {
            grenadeButton.interactable = false;
            grenadeButton.transform.GetChild(1).GetComponent<Image>().sprite = grenadeSprite;
            grenadeButton.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = baseSize;
            grenadeButton.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = "0";
        }
    }
}