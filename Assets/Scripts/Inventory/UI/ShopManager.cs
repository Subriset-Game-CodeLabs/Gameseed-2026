using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Item Database (auto-loaded from Resources if empty)")]
    [SerializeField] private List<BaseItem> allItems = new List<BaseItem>();

    [Header("Battle Loadout")]
    [SerializeField] private BattleInventory battleInventory;

    [Header("Scene Settings")]
    [SerializeField] private string battleSceneName = "BattleScene";
    [SerializeField] private string closeSceneName = "BattleScene";
    [SerializeField] private int battleSceneIndex = -1;

    // Auto-wired references
    [SerializeField] private Transform categoryButtonParent;
    [SerializeField] private Transform itemGridParent;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private ShopItemDetail itemDetail;
    [SerializeField] private TextMeshProUGUI moneyLabel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject shopPanel;

    // Equipment slots (bottom bar)
    private EquipmentSlot stickSlot;
    private EquipmentSlot[] skillSlots = new EquipmentSlot[3];

    private ItemCategory currentCategory;
    private List<GameObject> spawnedSlots = new List<GameObject>();

    // Public accessors for EquipmentSlot
    public BattleInventory BattleInventory => battleInventory;
    public ItemCategory CurrentCategory => currentCategory;

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

        // Clear battle inventory on shop open
        if (battleInventory != null)
            battleInventory.Clear();

        // Setup buttons
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseShop);
        }

        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(OnStartClicked);
        }

        // Setup category buttons (order: Jajan, Stick, Skill)
        SetupCategoryButtons();

        // Setup equipment slots
        SetupEquipmentSlots();

        // Open shop on first category
        gameObject.SetActive(true);
        SelectCategory(ItemCategory.UsableItem);

        // Force layout rebuild
        Canvas.ForceUpdateCanvases();
        if (shopPanel != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(shopPanel.GetComponent<RectTransform>());

        RefreshMoney();
        UpdateStartButton();
    }

    private void SetupCategoryButtons()
    {
        if (categoryButtonParent == null) return;

        ShopCategoryButton[] buttons = categoryButtonParent.GetComponentsInChildren<ShopCategoryButton>(true);

        // Order: UsableItem (Jajan), Stick, Skill
        ItemCategory[] categoryOrder = { ItemCategory.UsableItem, ItemCategory.Stick, ItemCategory.Skill };

        for (int i = 0; i < buttons.Length && i < categoryOrder.Length; i++)
        {
            buttons[i].Setup(categoryOrder[i], this);
        }
    }

    private void SetupEquipmentSlots()
    {
        // Find equipment slots by name
        GameObject root = shopPanel != null ? shopPanel : gameObject;
        Transform rootT = root.transform;

        // Find StickSlot
        Transform stickT = FindDeepUnder(rootT, "StickSlot");
        if (stickT != null)
        {
            stickSlot = stickT.GetComponent<EquipmentSlot>();
            if (stickSlot == null)
                stickSlot = stickT.gameObject.AddComponent<EquipmentSlot>();
            stickSlot.Setup(this, EquipmentSlotType.Stick, 0);
        }

        // Find SkillSlots
        for (int i = 0; i < 3; i++)
        {
            Transform skillT = FindDeepUnder(rootT, "SkillSlot_" + (i + 1));
            if (skillT == null)
                skillT = FindDeepUnder(rootT, "SkillSlot" + (i + 1));
            if (skillT == null)
                skillT = FindDeepUnder(rootT, "SkillSlot");

            if (skillT != null)
            {
                skillSlots[i] = skillT.GetComponent<EquipmentSlot>();
                if (skillSlots[i] == null)
                    skillSlots[i] = skillT.gameObject.AddComponent<EquipmentSlot>();
                skillSlots[i].Setup(this, EquipmentSlotType.Skill, i);
            }
        }
    }

    private void OnEnable()
    {
        if (moneyLabel != null)
            RefreshMoney();
    }

    // ---- Auto-wire references ----
    private void AutoWireReferences()
    {
        Transform shopPanelTransform = null;

        // First try: sibling at scene root (Manager and ShopPanel are siblings)
        UnityEngine.SceneManagement.Scene scene = gameObject.scene;
        UnityEngine.GameObject[] rootObjs = scene.GetRootGameObjects();
        for (int i = 0; i < rootObjs.Length; i++)
        {
            if (rootObjs[i].name == "ShopPanel")
            {
                shopPanelTransform = rootObjs[i].transform;
                break;
            }
        }

        // Fallback: look in parent or deep search
        if (shopPanelTransform == null && transform.parent != null)
            shopPanelTransform = transform.parent.Find("ShopPanel");
        if (shopPanelTransform == null)
            shopPanelTransform = FindDeep("ShopPanel");

        shopPanel = shopPanelTransform != null ? shopPanelTransform.gameObject : gameObject;

        // Header
        Transform header = FindDeepUnder(shopPanelTransform, "TopBar");
        if (header != null)
        {
            moneyLabel = FindChildDeep<TextMeshProUGUI>(header, "MoneyText");
            closeButton = FindChildDeep<Button>(shopPanelTransform, "CloseButton");
            startButton = FindChildDeep<Button>(shopPanelTransform, "StartButton");
        }

        // Category buttons
        categoryButtonParent = FindDeepUnder(shopPanelTransform, "CategoryButtonContainer");

        // Item Grid - Content inside ScrollView
        Transform scrollView = FindDeepUnder(shopPanelTransform, "ItemScrollView");
        if (scrollView != null)
        {
            itemGridParent = scrollView.Find("Viewport/Content");
            if (itemGridParent == null)
                itemGridParent = scrollView.Find("Viewport");
        }

        // ItemSlotPrefab
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

    // ---- Hierarchy search helpers ----
    private Transform FindDeep(string name)
    {
        return transform.Find(name) ?? FindDeepIn(transform, name);
    }

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

    private T FindChildDeep<T>(Transform parent, string name) where T : Component
    {
        Transform child = FindDeepIn(parent, name);
        if (child != null) return child.GetComponent<T>();
        return null;
    }

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

    // ---- Category Selection ----
    public void SelectCategory(ItemCategory category)
    {
        currentCategory = category;

        // Update tab highlight
        if (categoryButtonParent != null)
        {
            ShopCategoryButton[] buttons = categoryButtonParent.GetComponentsInChildren<ShopCategoryButton>(true);
            foreach (ShopCategoryButton btn in buttons)
                btn.SetSelected(btn.Category == category);
        }

        PopulateItemGrid(category);

        if (itemDetail != null)
            itemDetail.ShowNoSelection();
    }

    // ---- Item Grid ----
    private void PopulateItemGrid(ItemCategory category)
    {
        ClearItemGrid();

        if (itemSlotPrefab == null || itemGridParent == null)
        {
            Debug.LogWarning("[ShopManager] itemSlotPrefab or itemGridParent null!");
            return;
        }

        List<BaseItem> categoryItems = GetItemsByCategory(category);

        foreach (BaseItem item in categoryItems)
        {
            GameObject slotObj = Instantiate(itemSlotPrefab, itemGridParent);
            slotObj.SetActive(true);
            ShopItemSlot slot = slotObj.GetComponent<ShopItemSlot>();

            bool isOwned = false;
            int quantity = 0;
            if (InventoryManager.Instance != null && InventoryManager.Instance.Inventory != null)
            {
                isOwned = InventoryManager.Instance.Inventory.IsItemUnlocked(item);
                quantity = InventoryManager.Instance.Inventory.GetItemCount(item);
            }

            bool isSelected = false;
            if (battleInventory != null)
            {
                if (item is StickItem stickItem)
                    isSelected = battleInventory.selectedStick == stickItem;
                else if (item is SkillItem skillItem)
                    isSelected = battleInventory.IsSkillSelected(skillItem);
            }

            if (slot != null)
                slot.Setup(item, this, isOwned, quantity, isSelected);

            spawnedSlots.Add(slotObj);
        }

        FixContentSize();
    }

    private void FixContentSize()
    {
        if (itemGridParent == null) return;

        RectTransform contentRT = itemGridParent.GetComponent<RectTransform>();
        if (contentRT == null) return;

        GridLayoutGroup glg = contentRT.GetComponent<GridLayoutGroup>();
        if (glg == null) return;

        int itemCount = 0;
        for (int i = 0; i < contentRT.childCount; i++)
        {
            if (contentRT.GetChild(i).gameObject.activeSelf)
                itemCount++;
        }

        if (itemCount == 0)
        {
            contentRT.sizeDelta = new Vector2(0, 0);
            return;
        }

        int cols = glg.constraintCount;
        int rows = Mathf.CeilToInt((float)itemCount / (float)cols);
        float totalHeight = rows * glg.cellSize.y + (rows - 1) * glg.spacing.y + glg.padding.top + glg.padding.bottom;

        contentRT.sizeDelta = new Vector2(0, totalHeight);
        contentRT.anchoredPosition = Vector2.zero;

        ScrollRect scrollRect = itemGridParent.GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    // ---- Item Selection ----
    public void SelectItem(BaseItem item)
    {
        foreach (GameObject slotObj in spawnedSlots)
        {
            ShopItemSlot slot = slotObj.GetComponent<ShopItemSlot>();
            if (slot != null)
                slot.SetSelected(slot.Item == item);
        }

        if (itemDetail != null)
            itemDetail.ShowItem(item, this);
    }

    // ---- Equipment: Select Stick ----
    public void SelectStick(StickItem stick)
    {
        if (battleInventory == null) return;
        battleInventory.selectedStick = stick;

        RefreshEquipmentSlots();
        RefreshShop();
    }

    // ---- Equipment: Add Skill ----
    public bool AddSkill(SkillItem skill)
    {
        if (battleInventory == null) return false;
        int slotIndex = battleInventory.AddSkill(skill);
        if (slotIndex >= 0)
        {
            RefreshEquipmentSlots();
            RefreshShop();
            UpdateStartButton();
            return true;
        }
        return false;
    }

    // ---- Equipment: Remove Skill at index ----
    public void RemoveSkillAt(int index)
    {
        if (battleInventory == null) return;
        battleInventory.RemoveSkillAt(index);

        RefreshEquipmentSlots();
        RefreshShop();
        UpdateStartButton();
    }

    // ---- Refresh all equipment slot visuals ----
    public void RefreshEquipmentSlots()
    {
        // Stick slot
        if (stickSlot != null)
        {
            if (battleInventory != null && battleInventory.selectedStick != null)
                stickSlot.ShowItem(battleInventory.selectedStick.icon, battleInventory.selectedStick.itemName);
            else
                stickSlot.ShowEmpty();
        }

        // Skill slots
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (skillSlots[i] == null) continue;
            if (battleInventory != null && battleInventory.selectedSkills[i] != null)
            {
                SkillItem skill = battleInventory.selectedSkills[i];
                skillSlots[i].ShowItem(skill.icon, skill.itemName);
            }
            else
            {
                skillSlots[i].ShowEmpty();
            }
        }
    }

    // ---- Buy / Select logic (called from ShopItemDetail) ----
    public void BuyItem(BaseItem item)
    {
        if (InventoryManager.Instance == null) return;

        if (InventoryManager.Instance.BuyItem(item))
        {
            RefreshShop();
            RefreshMoney();

            // Update detail if same item still selected
            if (itemDetail != null)
                itemDetail.RefreshButtons();
        }
    }

    public void SelectStickItem(StickItem stick)
    {
        if (battleInventory == null) return;
        battleInventory.selectedStick = stick;
        RefreshEquipmentSlots();
        RefreshShop();
    }

    public void SelectSkillItem(SkillItem skill)
    {
        if (battleInventory == null) return;
        if (battleInventory.IsSkillSelected(skill))
        {
            // Deselect — find and remove
            int idx = battleInventory.GetSkillIndex(skill);
            if (idx >= 0)
                battleInventory.RemoveSkillAt(idx);
        }
        else
        {
            battleInventory.AddSkill(skill);
        }
        RefreshEquipmentSlots();
        RefreshShop();
        UpdateStartButton();
    }

    // ---- Start / Back ----
    private void OnStartClicked()
    {
        if (battleInventory == null || !battleInventory.AreAllSkillsSelected()) return;
        if (battleInventory.selectedStick == null) return;

        // Usable items are accessed directly from InventoryManager during battle
        if (battleSceneIndex >= 0)
            SceneManager.LoadScene(battleSceneIndex);
        else if (!string.IsNullOrEmpty(battleSceneName))
            SceneManager.LoadScene(battleSceneName);
    }

    public void CloseShop()
    {
        if (shopPanel != null)
            SceneManager.LoadScene(closeSceneName);
    }

    private void UpdateStartButton()
    {
        if (startButton == null) return;
        bool ready = battleInventory != null && battleInventory.selectedStick != null && battleInventory.AreAllSkillsSelected();
        startButton.interactable = ready;
    }

    // ---- Refresh ----
    public void RefreshShop()
    {
        PopulateItemGrid(currentCategory);
        RefreshMoney();
    }

    public void RefreshMoney()
    {
        if (moneyLabel == null) return;
        if (InventoryManager.Instance != null && InventoryManager.Instance.Inventory != null)
            moneyLabel.text = "RP" + InventoryManager.Instance.Inventory.money.ToString("N0");
    }

    public void OpenShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);
        SelectCategory(ItemCategory.UsableItem);
        RefreshMoney();
        RefreshEquipmentSlots();
        UpdateStartButton();
    }

    // ---- Helpers ----
    private void ClearItemGrid()
    {
        spawnedSlots.Clear();

        // Destroy all children immediately
        if (itemGridParent != null)
        {
            for (int i = itemGridParent.childCount - 1; i >= 0; i--)
            {
                Transform child = itemGridParent.GetChild(i);
                if (child != null)
                    DestroyImmediate(child.gameObject);
            }
        }
    }

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
}
