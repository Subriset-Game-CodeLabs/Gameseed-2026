using UnityEngine;

[CreateAssetMenu(fileName = "New Stick", menuName = "Inventory/Stick Item")]
public class StickItem : BaseItem
{
    [Header("Stick Data")]
    public GameObject stickPrefab;      // Prefab 3D stick yang digunakan saat bertarung
    public int damage = 1;
    public float throwForce = 10f;

    private void OnValidate()
    {
        category = ItemCategory.Stick;
    }
}
