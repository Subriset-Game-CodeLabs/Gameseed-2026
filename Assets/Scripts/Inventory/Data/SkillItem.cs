using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Inventory/Skill Item")]
public class SkillItem : BaseItem
{
    [Header("Skill Data")]
    public int cooldown = 5;
    public int manaCost = 10;
    public GameObject skillEffectPrefab;

    public SkillTarget targetType;
    public EffectSO[] effects; 

    private void OnValidate()
    {
        category = ItemCategory.Skill;
        itemType = ItemType.Unlockable;
    }
}
