using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerExitHandler
{
    public ItemType slotType;

    [SerializeField] private GameObject itemPrefabs;

    [SerializeField] private DragAndDrop dragAndDrop;
    [HideInInspector] public Item item;
    [HideInInspector] public Inventory inventory;
    public Vector2 infoPosition;
    public bool active;

    private Image slotTypeSprite;
    private Image slotSprite;
    private Text numberText;

    [HideInInspector] public int id;

    private Color32 nullColor = new Color32(255, 255, 255, 0);
    private Color32 activeColor = new Color32(255, 255, 255, 255);
    private Color32 personColor = new Color32(255, 255, 255, 100);

    public GameObject ItemPrefabs
    {
        get
        {
            return itemPrefabs;
        }
    }

    public void UpdateStot(bool Trigger = true)
    {
        //Один раз проходит
        if(slotSprite == null)
        {
            if (transform.parent.name == "Slots" || transform.parent.name == "Backpack Slots" || transform.parent.name == "Person Slots" || transform.parent.name == "Craft Slots")
            {
                slotSprite = transform.GetChild(0).GetChild(0).GetComponent<Image>();
                slotTypeSprite = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>();
                numberText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
            }
        }

        dragAndDrop.UpdateStats(this);

        if (item != null)
        {
            if(item.itemSprite != null)
            {
                slotSprite.sprite = item.itemSprite;
            }
            else
            {
                for (int i = 0; i < itemPrefabs.transform.childCount; i++)
                {
                    Item item = itemPrefabs.transform.GetChild(i).GetComponent<Item>();
                    if (item.itemName == this.item.itemName)
                    {
                        slotSprite.sprite = item.itemSprite;
                        if(this.item.itemType == ItemType.Grenade || this.item.itemType == ItemType.Weapon)
                        {
                            if(this.item.GetComponent<WeaponItem>().projectile == null)
                            {
                                this.item.GetComponent<WeaponItem>().projectile = item.GetComponent<WeaponItem>().projectile;
                            }
                        }
                        break;
                    }
                }
            }
            slotTypeSprite.sprite = item.itemTypeSprite;
            numberText.text = item.number.ToString();

            slotSprite.color = activeColor;

            if(slotType != ItemType.Other)
            {
                slotTypeSprite.color = nullColor;
            }
            else
            {
                if(slotTypeSprite.sprite.name == "!")
                {
                    slotTypeSprite.color = Color.yellow;
                }
                else
                {
                    slotTypeSprite.color = activeColor;
                }
            }

            if (item.stack)
            {
                numberText.color = activeColor;
            }
            else
            {
                numberText.color = nullColor;
            }
        }
        else
        {
            slotSprite.color = nullColor;

            if (slotType != ItemType.Other)
            {
                slotTypeSprite.color = personColor;
            }
            else
            {
                slotTypeSprite.color = nullColor;
            }

            numberText.color = nullColor;
        }

        if (slotType == ItemType.Weapon)
        {
            if(Trigger)
            {
                dragAndDrop.NewStats(this, ItemType.Weapon);
            }
        }
        else if(slotType == ItemType.Armor)
        {
            dragAndDrop.NewStats(this, ItemType.Armor);
        }
        else if (slotType == ItemType.Grenade)
        {
            inventory.transform.parent.GetComponent<GrenadeUIManager>().UpdateButton();
        }
        else if(slotType == ItemType.Medkit)
        {
            inventory.transform.parent.GetComponent<HealthSystem>().UpdateButton();
        }
        else if(slotType == ItemType.Backpack)
        {
            if(item != null)
            {
                for (int i = 0; i < item.GetComponent<BackpackItem>().dopSlots; i++)
                {
                    inventory.slots[i].active = true;
                    inventory.slots[i].UpdateStot();
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    inventory.slots[i].active = true;
                    inventory.slots[i].UpdateStot();
                }
            }
            inventory.transform.parent.GetComponent<Character>().UpdateBackpack();
        }

        if (active)
        {
            transform.GetChild(0).GetComponent<Image>().color = activeColor;
            if(transform.GetChild(0).GetChild(0).childCount > 2)
            {
                transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
            }
            RayCast(true);
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().color = personColor;
            if (transform.GetChild(0).GetChild(0).childCount > 2)
            {
                transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);
            }
            RayCast(false);
        }
    }

    public void ActiveSlot(bool value)
    {
        if (slotSprite == null)
        {
            slotSprite = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        }

        if(item != null)
        {
            if (item.itemSprite != null)
            {
                slotSprite.sprite = item.itemSprite;
            }
            else
            {
                for (int i = 0; i < itemPrefabs.transform.childCount; i++)
                {
                    Item item = itemPrefabs.transform.GetChild(i).GetComponent<Item>();
                    if (item.itemName == this.item.itemName)
                    {
                        slotSprite.sprite = item.itemSprite;
                        break;
                    }
                }
            }
        }

        if (value)
        {
            if(name != "Drag Slot")
            {
                slotTypeSprite.color = nullColor;
                numberText.color = nullColor;
            }
            slotSprite.color = nullColor;
        }
        else
        {
            if(name != "Drag Slot")
            {
                if(slotType != ItemType.Other)
                {
                    slotTypeSprite.color = nullColor;
                }
                else
                {
                    if (slotTypeSprite.sprite.name == "!")
                    {
                        slotTypeSprite.color = Color.yellow;
                    }
                    else
                    {
                        slotTypeSprite.color = activeColor;
                    }
                }

                if (item == null)
                {
                    numberText.color = nullColor;
                }
                else if (item.stack)
                {
                    numberText.color = activeColor;
                }
            }
            slotSprite.color = activeColor;
        }
    }

    public void SelectSpriteColor(Color color)
    {
        slotSprite.color = color;
    }

    public void RayCast(bool value)
    {
        GetComponent<Image>().raycastTarget = value;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        dragAndDrop.OnPointerEnter(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragAndDrop.OnBeginDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragAndDrop.OnDrag();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragAndDrop.OnEndDrag();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        dragAndDrop.OnPointerExit();
    }
}