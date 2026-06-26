using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EquipmentSlotType
{
    Stick,
    Skill
}

public class EquipmentSlot : MonoBehaviour
{
    private Image iconImage;
    private TextMeshProUGUI nameLabel;
    private TextMeshProUGUI slotLabel;
    private Button removeButton;
    private GameObject emptyIndicator;

    private ShopManager shopManager;
    private EquipmentSlotType slotType;
    private int slotIndex;

    public void Setup(ShopManager shop, EquipmentSlotType type, int index)
    {
        shopManager = shop;
        slotType = type;
        slotIndex = index;

        // Auto-wire children
        iconImage = FindChild<Image>("SlotIcon");
        nameLabel = FindChild<TextMeshProUGUI>("SlotName");
        slotLabel = FindChild<TextMeshProUGUI>("SlotLabel");
        removeButton = FindChild<Button>("RemoveButton");

        // Setup remove button (X) — only for skill slots
        if (removeButton != null)
        {
            removeButton.onClick.RemoveAllListeners();
            if (slotType == EquipmentSlotType.Skill)
                removeButton.onClick.AddListener(OnRemoveClicked);

            // Only show remove button for skill slots
            removeButton.gameObject.SetActive(slotType == EquipmentSlotType.Skill);
        }

        // Set slot label
        if (slotLabel != null)
        {
            if (slotType == EquipmentSlotType.Stick)
                slotLabel.text = "STICK";
            else
                slotLabel.text = "SKILL " + (slotIndex + 1);
        }

        ShowEmpty();
    }

    public void ShowItem(Sprite icon, string itemName)
    {
        if (iconImage != null)
        {
            if (icon != null)
            {
                iconImage.sprite = icon;
                iconImage.color = Color.white;
                iconImage.preserveAspect = true;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                iconImage.gameObject.SetActive(false);
            }
        }

        // Hide the text name — show sprite instead
        if (nameLabel != null)
            nameLabel.gameObject.SetActive(false);
    }

    public void ShowEmpty()
    {
        if (iconImage != null)
            iconImage.gameObject.SetActive(false);

        if (nameLabel != null)
            nameLabel.gameObject.SetActive(false);
    }

    private void OnRemoveClicked()
    {
        if (shopManager != null && slotType == EquipmentSlotType.Skill)
        {
            shopManager.RemoveSkillAt(slotIndex);
        }
    }

    private T FindChild<T>(string childName) where T : Component
    {
        Transform t = transform.Find(childName);
        if (t != null) return t.GetComponent<T>();
        return null;
    }
}
