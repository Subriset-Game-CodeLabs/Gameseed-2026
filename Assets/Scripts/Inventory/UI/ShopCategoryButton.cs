using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopCategoryButton : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI label;
    private GameObject selectedHighlight;

    private ItemCategory category;
    private ShopManager shopManager;

    private void Awake()
    {
        // Auto-wire
        button = GetComponent<Button>();
        if (button == null) button = GetComponentInChildren<Button>(true);
        label = GetComponentInChildren<TextMeshProUGUI>(true);

        // Find highlight child
        Transform hl = transform.Find("SelectedHighlight");
        if (hl != null) selectedHighlight = hl.gameObject;
    }

    public void Setup(ItemCategory category, ShopManager shopManager)
    {
        this.category = category;
        this.shopManager = shopManager;

        if (label != null)
            label.text = GetCategoryDisplayName(category);

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
            shopManager.SelectCategory(category);
    }

    private string GetCategoryDisplayName(ItemCategory cat)
    {
        switch (cat)
        {
            case ItemCategory.Stick: return "STICK";
            case ItemCategory.Skill: return "SKILL";
            case ItemCategory.UsableItem: return "USABLE";
            default: return cat.ToString();
        }
    }
}
