using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Inventory/Skill Item")]
public class SkillItem : BaseItem
{
    [Header("Skill Data")]
    public float cooldown = 5f;
    public int manaCost = 10;
    public GameObject skillEffectPrefab;

    private void OnValidate()
    {
        category = ItemCategory.Skill;
        itemType = ItemType.Unlockable;
    }
}
