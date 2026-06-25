using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Item Database (auto-loaded from Resources if empty)")]
    [SerializeField] private List<BaseItem> allItems = new List<BaseItem>();

    // Auto-wired references (found by name in hierarchy)
    private Transform categoryButtonParent;
    private Transform itemGridParent;
    private GameObject itemSlotPrefab;
    private ShopItemDetail itemDetail;
    private TextMeshProUGUI moneyLabel;
    private TextMeshProUGUI categoryTitle;
    private Button closeButton;
    private GameObject shopPanel;

    private ItemCategory currentCategory;
    private List<GameObject> spawnedSlots = new List<GameObject>();

    private void Awake()
    {
        AutoWireReferences();
    }

    private void Start()
    {
        // Load items from Resources if list is empty
        if (allItems == null || allItems.Count == 0)
        {
            allItems = new List<BaseItem>();
            BaseItem[] loaded = Resources.LoadAll<BaseItem>("Items");
            allItems.AddRange(loaded);
            if (loaded.Length == 0)
            {
                BaseItem[] loaded2 = Resources.LoadAll<BaseItem>("");
                foreach (BaseItem item in loaded2)
                {
                    if (!allItems.Contains(item))
                        allItems.Add(item);
                }
            }
        }

        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseShop);
        }

        // Wire up existing category buttons (already in scene)
        SetupExistingCategoryButtons();

        // Clean any old cloned item slots
        CleanOldClones();

        // Open shop on first category
        gameObject.SetActive(true);
        SelectCategory(ItemCategory.Stick);
    }

    private void SetupExistingCategoryButtons()
    {
        if (categoryButtonParent == null) return;

        // Find all existing ShopCategoryButton components
        ShopCategoryButton[] buttons = categoryButtonParent.GetComponentsInChildren<ShopCategoryButton>(true);

        if (buttons.Length == 0)
        {
            // If no buttons exist, create them from existing children with "CatBtn" prefix
            Transform[] children = new Transform[categoryButtonParent.childCount];
            for (int i = 0; i < children.Length; i++)
                children[i] = categoryButtonParent.GetChild(i);

            ItemCategory[] categories = { ItemCategory.Stick, ItemCategory.Skill, ItemCategory.UsableItem };
            for (int i = 0; i < children.Length && i < categories.Length; i++)
            {
                ShopCategoryButton catBtn = children[i].GetComponent<ShopCategoryButton>();
                if (catBtn == null)
                    catBtn = children[i].gameObject.AddComponent<ShopCategoryButton>();
                catBtn.Setup(categories[i], this);
            }
        }
        else
        {
            // Wire up existing buttons
            foreach (ShopCategoryButton btn in buttons)
            {
                // Determine category from name
                ItemCategory cat = ItemCategory.Stick;
                if (btn.name.Contains("SKILL")) cat = ItemCategory.Skill;
                else if (btn.name.Contains("USABLE")) cat = ItemCategory.UsableItem;
                btn.Setup(cat, this);
            }
        }
    }

    private void CleanOldClones()
    {
        if (itemGridParent == null) return;
        for (int i = itemGridParent.childCount - 1; i >= 0; i--)
        {
            Transform child = itemGridParent.GetChild(i);
            if (child.name.Contains("(Clone)") || child.name == "ItemSlotPrefab")
            {
                // Keep ItemSlotPrefab template, destroy clones
                if (child.name.Contains("(Clone)"))
                    Destroy(child.gameObject);
            }
        }
    }

    private void OnEnable()
    {
        if (moneyLabel != null)
            RefreshMoney();
    }

    // Auto-find all UI references by searching the hierarchy by name
    private void AutoWireReferences()
    {
        // ShopManager is at ShopCanvas/ShopManager (sibling of ShopPanel)
        // Find ShopPanel via parent
        Transform shopPanelTransform = null;
        if (transform.parent != null)
            shopPanelTransform = transform.parent.Find("ShopPanel");
        if (shopPanelTransform == null)
            shopPanelTransform = FindDeep("ShopPanel");

        shopPanel = shopPanelTransform != null ? shopPanelTransform.gameObject : gameObject;

        // Header - find under ShopPanel
        Transform header = FindDeepUnder(shopPanelTransform, "Header");
        if (header != null)
        {
            moneyLabel = FindChildDeep<TextMeshProUGUI>(header, "MoneyText");
            closeButton = FindChildDeep<Button>(header, "CloseButton");
        }

        // GridTitle is under ItemGridPanel
        Transform gridPanel = FindDeepUnder(shopPanelTransform, "ItemGridPanel");
        if (gridPanel != null)
            categoryTitle = FindChildDeep<TextMeshProUGUI>(gridPanel, "GridTitle");

        // Category Panel
        categoryButtonParent = FindDeepUnder(shopPanelTransform, "CategoryButtonContainer");

        // Item Grid - Content inside ScrollView
        Transform scrollView = FindDeepUnder(shopPanelTransform, "ItemScrollView");
        if (scrollView != null)
        {
            itemGridParent = scrollView.Find("Viewport/Content");
            if (itemGridParent == null)
                itemGridParent = scrollView.Find("Viewport");
        }

        // ItemSlotPrefab - find the deactivated template anywhere in scene
        itemSlotPrefab = FindInactive("ItemSlotPrefab");

        // Detail Panel
        Transform detailPanel = FindDeepUnder(shopPanelTransform, "DetailPanel");
        if (detailPanel != null)
        {
            itemDetail = detailPanel.GetComponent<ShopItemDetail>();
            if (itemDetail == null)
                itemDetail = detailPanel.gameObject.AddComponent<ShopItemDetail>();
        }
    }

    // Find a child recursively by name (active only)
    private Transform FindDeep(string name)
    {
        return transform.Find(name) ?? FindDeepIn(transform, name);
    }

    // Find under a specific root
    private Transform FindDeepUnder(Transform root, string name)
    {
        if (root == null) return null;
        Transform found = root.Find(name);
        if (found != null) return found;
        return FindDeepIn(root, name);
    }

    private Transform FindDeepIn(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = FindDeepIn(child, name);
            if (result != null) return result;
        }
        return null;
    }

    // Find a child component recursively by name
    private T FindChildDeep<T>(Transform parent, string name) where T : Component
    {
        Transform child = FindDeepIn(parent, name);
        if (child != null) return child.GetComponent<T>();
        return null;
    }

    // Find inactive GameObject by name
    private GameObject FindInactive(string name)
    {
        GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject go in all)
        {
            if (go.name == name && go.scene.IsValid())
                return go;
        }
        return null;
    }

    // Pilih kategori
    public void SelectCategory(ItemCategory category)
    {
        currentCategory = category;

        // Update highlight tombol kategori
        if (categoryButtonParent != null)
        {
            ShopCategoryButton[] buttons = categoryButtonParent.GetComponentsInChildren<ShopCategoryButton>(true);
            foreach (ShopCategoryButton btn in buttons)
                btn.SetSelected(btn.name.Contains(category.ToString()));
        }

        // Update judul
        if (categoryTitle != null)
            categoryTitle.text = GetCategoryDisplayName(category);

        // Populate item grid
        PopulateItemGrid(category);

        // Reset detail
        if (itemDetail != null)
            itemDetail.ShowNoSelection();
    }

    // Populate grid item di panel tengah
    private void PopulateItemGrid(ItemCategory category)
    {
        // Hapus slot lama
        ClearItemGrid();

        if (itemSlotPrefab == null || itemGridParent == null)
        {
            Debug.LogWarning("[ShopManager] itemSlotPrefab atau itemGridParent null!");
            return;
        }

        // Filter item berdasarkan kategori
        List<BaseItem> categoryItems = GetItemsByCategory(category);

        foreach (BaseItem item in categoryItems)
        {
            GameObject slotObj = Instantiate(itemSlotPrefab, itemGridParent);
            slotObj.SetActive(true);
            ShopItemSlot slot = slotObj.GetComponent<ShopItemSlot>();

            bool isOwned = false;
            if (InventoryManager.Instance != null && InventoryManager.Instance.Inventory != null)
                isOwned = InventoryManager.Instance.Inventory.IsItemUnlocked(item);

            if (slot != null)
                slot.Setup(item, this, isOwned);

            spawnedSlots.Add(slotObj);
        }
    }

    // Pilih item untuk ditampilkan di detail
    public void SelectItem(BaseItem item)
    {
        // Update highlight pada slot
        foreach (GameObject slotObj in spawnedSlots)
        {
            ShopItemSlot slot = slotObj.GetComponent<ShopItemSlot>();
            if (slot != null)
                slot.SetSelected(slot.Item == item);
        }

        // Tampilkan detail
        if (itemDetail != null)
            itemDetail.ShowItem(item);
    }

    // Refresh shop setelah beli
    public void RefreshShop()
    {
        PopulateItemGrid(currentCategory);
        RefreshMoney();
    }

    // Update tampilan uang
    public void RefreshMoney()
    {
        if (moneyLabel == null) return;
        if (InventoryManager.Instance != null && InventoryManager.Instance.Inventory != null)
            moneyLabel.text = InventoryManager.Instance.Inventory.money + " G";
    }

    // Hapus semua slot di grid
    private void ClearItemGrid()
    {
        foreach (GameObject slot in spawnedSlots)
        {
            if (slot != null) Destroy(slot);
        }
        spawnedSlots.Clear();
    }

    // Ambil item berdasarkan kategori dari database
    private List<BaseItem> GetItemsByCategory(ItemCategory category)
    {
        List<BaseItem> result = new List<BaseItem>();
        if (allItems == null) return result;

        foreach (BaseItem item in allItems)
        {
            if (item != null && item.category == category)
                result.Add(item);
        }

        return result;
    }

    // Nama kategori untuk display
    private string GetCategoryDisplayName(ItemCategory category)
    {
        switch (category)
        {
            case ItemCategory.Stick:      return "STICK";
            case ItemCategory.Skill:      return "SKILL";
            case ItemCategory.UsableItem: return "USABLE ITEM";
            default:                      return "";
        }
    }

    // Buka / Tutup shop
    public void OpenShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);
        SelectCategory(ItemCategory.Stick);
        RefreshMoney();
    }

    public void CloseShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }
}
