using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    private Button button;
    private Image iconImage;
    private TextMeshProUGUI nameLabel;
    private TextMeshProUGUI priceLabel;
    private GameObject lockedOverlay;
    private GameObject selectedHighlight;

    private BaseItem item;
    private ShopManager shopManager;

    public BaseItem Item => item;

    private void Awake()
    {
        // Auto-wire by finding children by name
        button = GetComponent<Button>();
        if (button == null) button = GetComponentInChildren<Button>(true);

        iconImage = FindChild<Image>("SlotIcon");
        nameLabel = FindChild<TextMeshProUGUI>("SlotName");
        priceLabel = FindChild<TextMeshProUGUI>("SlotPrice");
        lockedOverlay = FindChildGO("LockedOverlay");
        selectedHighlight = FindChildGO("SelectedHighlight");

        // If no name/price found, try to find any TMP in children
        if (nameLabel == null) nameLabel = GetComponentInChildren<TextMeshProUGUI>(true);
        if (button == null) button = gameObject.AddComponent<Button>();

        // Auto-add Image target to button if none set
        if (button != null && button.targetGraphic == null)
        {
            Image bg = GetComponent<Image>();
            if (bg != null) button.targetGraphic = bg;
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

    public void Setup(BaseItem item, ShopManager shopManager, bool isOwned)
    {
        this.item = item;
        this.shopManager = shopManager;

        // Set tampilan
        if (iconImage != null && item.icon != null)
        {
            iconImage.sprite = item.icon;
            iconImage.gameObject.SetActive(true);
        }

        if (nameLabel != null)
            nameLabel.text = item.itemName;

        if (priceLabel != null)
        {
            if (item.itemType == ItemType.Unlockable && isOwned)
                priceLabel.text = "OWNED";
            else
                priceLabel.text = item.price + " G";
        }

        // Tampilkan overlay jika belum dimiliki (unlockable)
        if (lockedOverlay != null)
            lockedOverlay.SetActive(item.itemType == ItemType.Unlockable && !isOwned);

        // Event
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    public void SetSelected(bool selected)
    {
        if (selectedHighlight != null)
            selectedHighlight.SetActive(selected);
    }

    private void OnClick()
    {
        if (shopManager != null)
            shopManager.SelectItem(item);
    }
}
