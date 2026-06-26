using UnityEngine;

public enum ItemCategory
{
    Stick,
    Skill,
    UsableItem
}

public enum ItemType
{
    Unlockable,   // Sekali beli, permanen
    Usable        // Bisa dibeli berulang, bisa habis
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Base Item")]
public abstract class BaseItem : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;
    public int price;

    [Header("Item Type")]
    public ItemCategory category;
    public ItemType itemType;
}
