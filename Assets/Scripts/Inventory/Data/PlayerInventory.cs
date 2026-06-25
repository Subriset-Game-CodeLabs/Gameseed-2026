using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryEntry
{
    public BaseItem item;
    public int quantity;

    public InventoryEntry(BaseItem item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Inventory/Player Inventory")]
public class PlayerInventory : ScriptableObject
{
    [Header("Currency")]
    public int money = 1000;

    [Header("Items")]
    public List<InventoryEntry> sticks = new List<InventoryEntry>();
    public List<InventoryEntry> skills = new List<InventoryEntry>();
    public List<InventoryEntry> usableItems = new List<InventoryEntry>();

    // Cek apakah item unlockable sudah dimiliki
    public bool IsItemUnlocked(BaseItem item)
    {
        List<InventoryEntry> list = GetListByCategory(item.category);
        foreach (InventoryEntry entry in list)
        {
            if (entry.item == item)
                return true;
        }
        return false;
    }

    // Dapatkan jumlah item usable
    public int GetItemCount(BaseItem item)
    {
        List<InventoryEntry> list = GetListByCategory(item.category);
        foreach (InventoryEntry entry in list)
        {
            if (entry.item == item)
                return entry.quantity;
        }
        return 0;
    }

    // Tambah item ke inventory
    public void AddItem(BaseItem item, int quantity = 1)
    {
        List<InventoryEntry> list = GetListByCategory(item.category);

        // Cari apakah sudah ada
        foreach (InventoryEntry entry in list)
        {
            if (entry.item == item)
            {
                entry.quantity += quantity;
                return;
            }
        }

        // Belum ada, tambah baru
        list.Add(new InventoryEntry(item, quantity));
    }

    // Kurangi item (untuk usable item yang dipakai)
    public bool RemoveItem(BaseItem item, int quantity = 1)
    {
        List<InventoryEntry> list = GetListByCategory(item.category);

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].item == item)
            {
                list[i].quantity -= quantity;
                if (list[i].quantity <= 0)
                    list.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    // Kurangi uang
    public bool SpendMoney(int amount)
    {
        if (money < amount)
            return false;
        money -= amount;
        return true;
    }

    // Tambah uang
    public void AddMoney(int amount)
    {
        money += amount;
    }

    // Ambil list berdasarkan kategori
    private List<InventoryEntry> GetListByCategory(ItemCategory category)
    {
        switch (category)
        {
            case ItemCategory.Stick:    return sticks;
            case ItemCategory.Skill:    return skills;
            case ItemCategory.UsableItem: return usableItems;
            default:                    return sticks;
        }
    }

    // Reset inventory (untuk debug / new game)
    public void ResetInventory()
    {
        money = 1000;
        sticks.Clear();
        skills.Clear();
        usableItems.Clear();
    }
}
