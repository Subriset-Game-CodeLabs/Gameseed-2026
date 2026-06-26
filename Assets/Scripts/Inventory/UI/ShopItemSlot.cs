using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    private Button button;
    private Image iconImage;
    private TextMeshProUGUI nameLabel;
    private TextMeshProUGUI qtyLabel;
    private Image borderImage;

    private BaseItem item;
    private ShopManager shopManager;
    private int quantity;
    private bool isSelected;

    public BaseItem Item => item;

    // Default slot colors
    private static readonly Color normalColor = new Color(0.85f, 0.82f, 0.75f, 1f);  // beige
    private static readonly Color selectedColor = new Color(0.3f, 0.7f, 0.3f, 1f);    // green border
    private static readonly Color lockedColor = new Color(0.6f, 0.58f, 0.55f, 1f);    // darker beige (locked)

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null) button = GetComponentInChildren<Button>(true);

        iconImage = FindChild<Image>("SlotIcon");
        nameLabel = FindChild<TextMeshProUGUI>("SlotName");
        qtyLabel = FindChild<TextMeshProUGUI>("SlotQty");
        borderImage = GetComponent<Image>();

        if (nameLabel == null) nameLabel = GetComponentInChildren<TextMeshProUGUI>(true);
        if (button == null) button = gameObject.AddComponent<Button>();

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

    public void Setup(BaseItem item, ShopManager shopManager, bool isOwned, int quantity, bool isSelected)
    {
        this.item = item;
        this.shopManager = shopManager;
        this.quantity = quantity;
        this.isSelected = isSelected;

        // Icon
        if (iconImage != null)
        {
            // Defensive: ensure icon is always fully visible and not stretched
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;

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

        // Name
        if (nameLabel != null)
            nameLabel.text = item.itemName;

        // Quantity (only for UsableItem)
        if (qtyLabel != null)
        {
            if (item.category == ItemCategory.UsableItem)
            {
                qtyLabel.gameObject.SetActive(true);
                qtyLabel.text = "Qty: " + quantity;
            }
            else
            {
                qtyLabel.gameObject.SetActive(false);
            }
        }

        // Border color
        UpdateBorderColor(isOwned, isSelected);

        // Button
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    private void UpdateBorderColor(bool isOwned, bool isSelected)
    {
        if (borderImage == null) return;

        if (isSelected)
            borderImage.color = selectedColor;
        else if (!isOwned && item.itemType == ItemType.Unlockable)
            borderImage.color = lockedColor;
        else
            borderImage.color = normalColor;
    }

    public void SetSelected(bool selected)
    {
        this.isSelected = selected;
        if (borderImage != null)
            borderImage.color = selected ? selectedColor : normalColor;
    }

    private void OnClick()
    {
        if (shopManager != null)
            shopManager.SelectItem(item);
    }
}
