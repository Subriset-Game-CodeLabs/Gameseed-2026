using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : PersistentSingleton<InventoryManager>
{
    [SerializeField] private PlayerInventory playerInventory;

    public PlayerInventory Inventory => playerInventory;

    // Event
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (playerInventory == null)
        {
            playerInventory = Resources.Load("PlayerInventory") as PlayerInventory;
            if (playerInventory != null)
                Debug.Log("[InventoryManager] Auto-loaded PlayerInventory from Resources.");
            else
                Debug.LogWarning("[InventoryManager] PlayerInventory not found in Resources!");
        }
    }

    // Beli item
    public bool BuyItem(BaseItem item)
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("PlayerInventory belum di-assign!");
            return false;
        }

        // Cek uang
        if (playerInventory.money < item.price)
        {
            Debug.Log("Uang tidak cukup!");
            return false;
        }

        // Cek unlockable
        if (item.itemType == ItemType.Unlockable && playerInventory.IsItemUnlocked(item))
        {
            Debug.Log("Item sudah dimiliki!");
            return false;
        }

        // Beli
        if (playerInventory.SpendMoney(item.price))
        {
            playerInventory.AddItem(item);
            OnInventoryChanged?.Invoke();
            Debug.Log($"Membeli {item.itemName} seharga {item.price}");
            return true;
        }

        return false;
    }

    // Gunakan item
    public bool UseItem(BaseItem item)
    {
        if (item.itemType != ItemType.Usable)
            return false;

        if (playerInventory.GetItemCount(item) <= 0)
            return false;

        Debug.Log("Item used");
        playerInventory.RemoveItem(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    // Ambil list berdasarkan kategori
    public List<InventoryEntry> GetItemsByCategory(ItemCategory category)
    {
        if (playerInventory == null) return new List<InventoryEntry>();

        switch (category)
        {
            case ItemCategory.Stick:    return playerInventory.sticks;
            case ItemCategory.Skill:    return playerInventory.skills;
            case ItemCategory.UsableItem: return playerInventory.usableItems;
            default:                    return playerInventory.sticks;
        }
    }

    // Ambil semua item dari database (semua ScriptableObject yang ada di project)
    public List<BaseItem> GetAllItemDatabase()
    {
        List<BaseItem> allItems = new List<BaseItem>();
        BaseItem[] items = Resources.FindObjectsOfTypeAll<BaseItem>();
        allItems.AddRange(items);
        return allItems;
    }

    // Ambil item berdasarkan kategori dari database
    public List<BaseItem> GetItemDatabaseByCategory(ItemCategory category)
    {
        List<BaseItem> result = new List<BaseItem>();
        BaseItem[] allItems = Resources.FindObjectsOfTypeAll<BaseItem>();

        foreach (BaseItem item in allItems)
        {
            if (item.category == category)
                result.Add(item);
        }

        return result;
    }

    // Cek apakah item bisa dibeli
    public bool CanBuyItem(BaseItem item)
    {
        if (playerInventory == null) return false;
        if (playerInventory.money < item.price) return false;
        if (item.itemType == ItemType.Unlockable && playerInventory.IsItemUnlocked(item)) return false;
        return true;
    }
}
