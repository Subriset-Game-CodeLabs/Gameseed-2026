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

        // Ensure Image has a sprite so Unity renders the button geometry
        Image img = GetComponent<Image>();
        if (img != null)
        {
            if (img.sprite == null)
            {
                Texture2D tex = new Texture2D(4, 4);
                Color[] px = new Color[16];
                for (int p = 0; p < 16; p++) px[p] = Color.white;
                tex.SetPixels(px);
                tex.Apply();
                img.sprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 100f);
            }

            // Set distinct visible colors per category
            switch (category)
            {
                case ItemCategory.Stick:
                    img.color = new Color(0.20f, 0.35f, 0.65f, 1f); // Blue
                    break;
                case ItemCategory.Skill:
                    img.color = new Color(0.25f, 0.55f, 0.25f, 1f); // Green
                    break;
                case ItemCategory.UsableItem:
                    img.color = new Color(0.70f, 0.40f, 0.15f, 1f); // Orange
                    break;
                default:
                    img.color = new Color(0.25f, 0.25f, 0.30f, 1f); // Dark gray
                    break;
            }
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
