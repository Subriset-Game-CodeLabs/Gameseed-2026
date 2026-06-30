using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _itemPrefab;

    public event Action<BaseItem> OnItemPressed;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetupUI()
    {
        List<InventoryEntry> itemList = InventoryManager.Instance.GetItemsByCategory(ItemCategory.UsableItem);

        for (var i = 0; i < itemList.Count; i++)
        {
            GameObject itemUI = Instantiate(_itemPrefab, transform);
            TMP_Text skillText = itemUI.GetComponentInChildren<TMP_Text>();
            skillText.text = itemList[i].quantity.ToString();
            Image skillImage = itemUI.GetComponent<Image>();
            skillImage.sprite = itemList[i].item.icon;
            Button skillButton = itemUI.GetComponent<Button>();

            int index = i;
            skillButton.onClick.AddListener(() =>
            {
                OnItemPressed?.Invoke(itemList[index].item);
            });

        }
    }

    public void RefreshUI()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        SetupUI();
    }
}
