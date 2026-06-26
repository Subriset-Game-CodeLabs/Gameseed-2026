using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemDetail : MonoBehaviour
{
    private TextMeshProUGUI nameLabel;
    private TextMeshProUGUI statLabel;
    private TextMeshProUGUI descriptionLabel;
    private TextMeshProUGUI qtyLabel;
    private Button chooseButton;
    private TextMeshProUGUI chooseButtonLabel;
    private GameObject noSelectionPanel;
    private Transform modelPreviewParent;

    private BaseItem currentItem;
    private ShopManager currentShop;
    private GameObject currentPreview;

    private void Awake()
    {
        AutoWireReferences();
    }

    private void Start()
    {
        if (chooseButton != null)
        {
            chooseButton.onClick.RemoveAllListeners();
            chooseButton.onClick.AddListener(OnChooseClicked);
        }
        ShowNoSelection();
    }

    private void AutoWireReferences()
    {
        nameLabel = FindChild<TextMeshProUGUI>("ItemName");
        statLabel = FindChild<TextMeshProUGUI>("ItemStat");
        descriptionLabel = FindChild<TextMeshProUGUI>("ItemDescription");
        qtyLabel = FindChild<TextMeshProUGUI>("ItemQty");
        chooseButton = FindChild<Button>("BuyButton");
        modelPreviewParent = FindChild<Transform>("ModelPreview");
        noSelectionPanel = FindChildGO("NoSelectionText");

        if (chooseButton != null)
        {
            Transform textChild = chooseButton.transform.Find("Text");
            if (textChild != null)
                chooseButtonLabel = textChild.GetComponent<TextMeshProUGUI>();
            if (chooseButtonLabel == null)
                chooseButtonLabel = chooseButton.GetComponentInChildren<TextMeshProUGUI>(true);
        }
    }

    private T FindChild<T>(string childName) where T : Component
    {
        Transform t = transform.Find(childName);
        if (t != null) return t.GetComponent<T>();
        return null;
    }

    private GameObject FindChildGO(string childName)
    {
        Transform t = transform.Find(childName);
        return t != null ? t.gameObject : null;
    }

    public void ShowItem(BaseItem item, ShopManager shop)
    {
        currentItem = item;
        currentShop = shop;

        if (noSelectionPanel != null)
            noSelectionPanel.SetActive(false);

        if (nameLabel != null)
            nameLabel.text = item.itemName;

        // Stat display based on type
        if (statLabel != null)
        {
            string stat = "";
            StickItem stick = item as StickItem;
            SkillItem skill = item as SkillItem;
            UsableItem usable = item as UsableItem;
            if (stick != null)
                stat = "Damage: " + stick.damage + "\nThrow: " + stick.throwForce;
            else if (skill != null)
                stat = "Cooldown: " + skill.cooldown + "s\nMana: " + skill.manaCost;
            else if (usable != null)
                stat = "+" + usable.effectValue + " " + usable.effectType.ToString();
            statLabel.text = stat;
        }

        if (descriptionLabel != null)
            descriptionLabel.text = item.description;

        // Quantity for usable items
        if (qtyLabel != null)
        {
            if (item.category == ItemCategory.UsableItem)
            {
                qtyLabel.gameObject.SetActive(true);
                int qty = 0;
                if (InventoryManager.Instance != null && InventoryManager.Instance.Inventory != null)
                    qty = InventoryManager.Instance.Inventory.GetItemCount(item);
                qtyLabel.text = "Stock: " + qty;
            }
            else
            {
                qtyLabel.gameObject.SetActive(false);
            }
        }

        Show3DPreview(item);
        UpdateChooseButton();
    }

    public void ShowNoSelection()
    {
        currentItem = null;
        currentShop = null;

        if (nameLabel != null) nameLabel.text = "";
        if (statLabel != null) statLabel.text = "";
        if (descriptionLabel != null) descriptionLabel.text = "";
        if (qtyLabel != null) qtyLabel.gameObject.SetActive(false);
        if (chooseButton != null) chooseButton.gameObject.SetActive(false);
        if (noSelectionPanel != null) noSelectionPanel.SetActive(true);

        Clear3DPreview();
    }

    private void Show3DPreview(BaseItem item)
    {
        Clear3DPreview();
        if (modelPreviewParent == null) return;

        StickItem stickItem = item as StickItem;
        if (stickItem != null && stickItem.stickPrefab != null)
        {
            currentPreview = Instantiate(stickItem.stickPrefab, modelPreviewParent);
            currentPreview.transform.localPosition = Vector3.zero;
            currentPreview.transform.localRotation = Quaternion.identity;
            Rigidbody rb = currentPreview.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
            Collider col = currentPreview.GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }

    private void Clear3DPreview()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }
    }

    public void RefreshButtons()
    {
        UpdateChooseButton();
    }

    private void UpdateChooseButton()
    {
        if (chooseButton == null || currentItem == null) return;
        chooseButton.gameObject.SetActive(true);

        InventoryManager manager = InventoryManager.Instance;
        if (manager == null) { chooseButtonLabel.text = "BUY"; chooseButton.interactable = false; return; }

        BattleInventory battleInv = currentShop != null ? currentShop.BattleInventory : null;
        bool isOwned = manager.Inventory != null ? manager.Inventory.IsItemUnlocked(currentItem) : false;
        bool canBuy = manager.Inventory != null ? manager.CanBuyItem(currentItem) : false;

        // UsableItem: always BUY
        if (currentItem.category == ItemCategory.UsableItem)
        {
            if (chooseButtonLabel != null)
                chooseButtonLabel.text = "BUY (" + currentItem.price + " G)";
            chooseButton.interactable = canBuy;
            return;
        }

        // Stick
        if (currentItem is StickItem stickItem)
        {
            if (!isOwned)
            {
                if (chooseButtonLabel != null)
                    chooseButtonLabel.text = "BUY (" + currentItem.price + " G)";
                chooseButton.interactable = canBuy;
            }
            else if (battleInv != null && battleInv.selectedStick == stickItem)
            {
                if (chooseButtonLabel != null)
                    chooseButtonLabel.text = "SELECTED";
                chooseButton.interactable = false;
            }
            else
            {
                if (chooseButtonLabel != null)
                    chooseButtonLabel.text = "SELECT";
                chooseButton.interactable = true;
            }
            return;
        }

        // Skill
        if (currentItem is SkillItem skillItem)
        {
            if (!isOwned)
            {
                if (chooseButtonLabel != null)
                    chooseButtonLabel.text = "BUY (" + currentItem.price + " G)";
                chooseButton.interactable = canBuy;
            }
            else if (battleInv != null && battleInv.IsSkillSelected(skillItem))
            {
                if (chooseButtonLabel != null)
                    chooseButtonLabel.text = "SELECTED";
                chooseButton.interactable = false;
            }
            else
            {
                if (chooseButtonLabel != null)
                    chooseButtonLabel.text = "SELECT";
                chooseButton.interactable = true;
            }
        }
    }

    private void OnChooseClicked()
    {
        if (currentItem == null || currentShop == null) return;

        InventoryManager manager = InventoryManager.Instance;
        if (manager == null) return;

        bool isOwned = manager.Inventory != null ? manager.Inventory.IsItemUnlocked(currentItem) : false;

        // UsableItem: always buy
        if (currentItem.category == ItemCategory.UsableItem)
        {
            currentShop.BuyItem(currentItem);
            return;
        }

        // Stick or Skill
        if (!isOwned)
        {
            // Buy first
            currentShop.BuyItem(currentItem);
            return;
        }

        // Already owned: select/deselect
        if (currentItem is StickItem stickItem)
        {
            currentShop.SelectStickItem(stickItem);
        }
        else if (currentItem is SkillItem skillItem)
        {
            currentShop.SelectSkillItem(skillItem);
        }

        UpdateChooseButton();
    }

    private void OnDestroy()
    {
        Clear3DPreview();
    }
}
