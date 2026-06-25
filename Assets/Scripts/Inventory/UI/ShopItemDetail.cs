using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemDetail : MonoBehaviour
{
    // Auto-wired references
    private Image iconImage;
    private TextMeshProUGUI nameLabel;
    private TextMeshProUGUI descriptionLabel;
    private TextMeshProUGUI priceLabel;
    private TextMeshProUGUI typeLabel;
    private Button buyButton;
    private TextMeshProUGUI buyButtonLabel;
    private Transform modelPreviewParent;
    private GameObject noSelectionPanel;

    private BaseItem currentItem;
    private GameObject currentPreview;

    private void Awake()
    {
        AutoWireReferences();
    }

    private void Start()
    {
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyClicked);
        }
        ShowNoSelection();
    }

    private void AutoWireReferences()
    {
        // Find children by actual hierarchy names
        iconImage = FindChild<Image>("ItemIcon");
        nameLabel = FindChild<TextMeshProUGUI>("ItemName");
        descriptionLabel = FindChild<TextMeshProUGUI>("ItemDescription");
        priceLabel = FindChild<TextMeshProUGUI>("ItemPrice");
        typeLabel = FindChild<TextMeshProUGUI>("ItemType");
        buyButton = FindChild<Button>("BuyButton");
        modelPreviewParent = FindChild<Transform>("ModelPreview");
        
        // NoSelectionText is a text, not a panel - use it as the no-selection indicator
        noSelectionPanel = FindChildGO("NoSelectionText");
        
        // If buyButton exists, find Text child for label
        if (buyButton != null)
        {
            Transform textChild = buyButton.transform.Find("Text");
            if (textChild != null)
                buyButtonLabel = textChild.GetComponent<TextMeshProUGUI>();
        }

        // Fallback: find first TMP in children if specific names not found
        if (nameLabel == null)
        {
            TextMeshProUGUI[] allTMP = GetComponentsInChildren<TextMeshProUGUI>(true);
            if (allTMP.Length > 0) nameLabel = allTMP[0];
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

    public void ShowItem(BaseItem item)
    {
        currentItem = item;

        if (noSelectionPanel != null)
            noSelectionPanel.SetActive(false);

        if (nameLabel != null)
            nameLabel.text = item.itemName;

        if (descriptionLabel != null)
            descriptionLabel.text = item.description;

        if (priceLabel != null)
            priceLabel.text = "Harga: " + item.price + " G";

        if (typeLabel != null)
        {
            string typeText = item.itemType == ItemType.Unlockable ? "Unlockable" : "Usable";
            typeLabel.text = "Tipe: " + typeText;
        }

        if (iconImage != null)
        {
            if (item.icon != null)
            {
                iconImage.sprite = item.icon;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                iconImage.gameObject.SetActive(false);
            }
        }

        Show3DPreview(item);
        UpdateBuyButton();
    }

    public void ShowNoSelection()
    {
        currentItem = null;

        if (nameLabel != null) nameLabel.text = "";
        if (descriptionLabel != null) descriptionLabel.text = "";
        if (priceLabel != null) priceLabel.text = "";
        if (typeLabel != null) typeLabel.text = "";
        if (iconImage != null) iconImage.gameObject.SetActive(false);
        if (buyButton != null) buyButton.gameObject.SetActive(false);
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

    private void UpdateBuyButton()
    {
        if (buyButton == null || currentItem == null) return;

        buyButton.gameObject.SetActive(true);

        InventoryManager manager = InventoryManager.Instance;
        if (manager == null) return;

        bool canBuy = manager.CanBuyItem(currentItem);

        if (currentItem.itemType == ItemType.Unlockable && manager.Inventory.IsItemUnlocked(currentItem))
        {
            if (buyButtonLabel != null) buyButtonLabel.text = "SUDAH DIMILIKI";
            buyButton.interactable = false;
        }
        else if (!canBuy)
        {
            if (buyButtonLabel != null) buyButtonLabel.text = "UANG TIDAK CUKUP";
            buyButton.interactable = false;
        }
        else
        {
            if (buyButtonLabel != null) buyButtonLabel.text = "BELI (" + currentItem.price + " G)";
            buyButton.interactable = true;
        }
    }

    private void OnBuyClicked()
    {
        if (currentItem == null) return;

        InventoryManager manager = InventoryManager.Instance;
        if (manager == null) return;

        if (manager.BuyItem(currentItem))
        {
            UpdateBuyButton();
            ShopManager shop = FindFirstObjectByType<ShopManager>();
            if (shop != null) shop.RefreshShop();
        }
    }

    private void OnDestroy()
    {
        Clear3DPreview();
    }
}
