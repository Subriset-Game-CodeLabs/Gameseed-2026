using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopCategoryButton : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI label;
    private Image backgroundImage;

    private ItemCategory category;
    private ShopManager shopManager;
    private bool isSelected;

    public ItemCategory Category => category;

    // Colors
    private static readonly Color normalBg = new Color(0.9f, 0.88f, 0.82f, 1f);  // light beige
    private static readonly Color selectedBg = new Color(1f, 1f, 1f, 1f);          // white when selected
    private static readonly Color normalText = new Color(0.4f, 0.38f, 0.35f, 1f); // dark text
    private static readonly Color selectedText = new Color(0.1f, 0.1f, 0.1f, 1f); // darker text

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null) button = GetComponentInChildren<Button>(true);
        label = GetComponentInChildren<TextMeshProUGUI>(true);
        backgroundImage = GetComponent<Image>();
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

        // Ensure sprite
        if (backgroundImage != null && backgroundImage.sprite == null)
        {
            Texture2D tex = new Texture2D(4, 4);
            Color[] px = new Color[16];
            for (int p = 0; p < 16; p++) px[p] = Color.white;
            tex.SetPixels(px);
            tex.Apply();
            backgroundImage.sprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 100f);
        }

        ApplyVisual(false);
    }

    public void SetSelected(bool selected)
    {
        this.isSelected = selected;
        ApplyVisual(selected);
    }

    private void ApplyVisual(bool selected)
    {
        if (backgroundImage != null)
            backgroundImage.color = selected ? selectedBg : normalBg;
        if (label != null)
            label.color = selected ? selectedText : normalText;
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
            case ItemCategory.UsableItem: return "JAJAN";
            default: return cat.ToString();
        }
    }
}
