using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Weapon,
    Ammo,
    Armor,
    Grenade,
    Medkit,
    Backpack,
    Other,
    Craft
}

public class Item : MonoBehaviour
{
    public int ID;

    public string itemName;
    public string englishItemName;
    public string indonesianItemName;

    public string itemDescription;
    public string englishItemDescription;
    public string indonesianItemDescription;

    public ItemType itemType;
    public bool stack;
    public float number;
    public int cost;
    public int weight;
    public Sprite itemTypeSprite;
    public Sprite itemSprite;
    public bool sell;
    public bool equipped;
    public int needStrength;

    public bool CheckAttribute(Attributes Attributes)
    {
        if (Attributes.Strength < needStrength)
        {
            return false;
        }
        return true;
    }
}
