using UnityEngine;

public enum UsableEffectType
{
    Heal,
    BuffAttack,
    BuffDefense,
    Revive,
    Other
}

[CreateAssetMenu(fileName = "New Usable Item", menuName = "Inventory/Usable Item")]
public class UsableItem : BaseItem
{
    [Header("Usable Data")]
    public UsableEffectType effectType;
    public int effectValue = 10;
    public int maxStack = 99;

    private void OnValidate()
    {
        category = ItemCategory.UsableItem;
        itemType = ItemType.Usable;
    }
}
