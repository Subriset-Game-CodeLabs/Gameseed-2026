using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelDetailPanel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject _panelRoot;

    [Header("Content")]
    [SerializeField] private Image _levelOverview;
    [SerializeField] private TextMeshProUGUI _levelNameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Image _enemyImage;

    [Header("Rewards")]
    [SerializeField] private Transform _rewardListParent;
    [SerializeField] private GameObject _rewardItemPrefab;

    [Header("Buttons")]
    [SerializeField] private Button _proceedButton;
    [SerializeField] private Button _closeButton;

    private LevelData _currentLevel;
    private GameObject _previouslySelected;
    private readonly List<GameObject> _spawnedRewardItems = new List<GameObject>();

    private void Awake()
    {
        _proceedButton.onClick.AddListener(OnProceedClicked);
        _closeButton.onClick.AddListener(Hide);
        _panelRoot.SetActive(false);
    }

    public void Show(LevelData level)
    {
        _currentLevel = level;

        _previouslySelected = EventSystem.current.currentSelectedGameObject;

        _levelNameText.SetText(level.LevelName);
        _descriptionText.SetText(level.Description);

        bool hasEnemy = level.EnemySprite != null;
        _enemyImage.enabled = hasEnemy;
        if (hasEnemy) _enemyImage.sprite = level.EnemySprite;

        bool hasOverview = level.LevelOverview != null;
        _levelOverview.enabled = hasOverview;
        if (hasOverview) _levelOverview.sprite = level.LevelOverview;

        PopulateRewards(level.Rewards);

        _panelRoot.SetActive(true);

        EventSystem.current.SetSelectedGameObject(_proceedButton.gameObject);
    }

    public void Hide()
    {
        _panelRoot.SetActive(false);
        ClearRewards();
        _currentLevel = null;

        if (_previouslySelected != null)
        {
            EventSystem.current.SetSelectedGameObject(_previouslySelected);
        }
    }

    private void PopulateRewards(List<RewardData> rewards)
    {
        ClearRewards();
        if (rewards == null) return;

        foreach (var reward in rewards)
        {
            GameObject item = Instantiate(_rewardItemPrefab, _rewardListParent);
            _spawnedRewardItems.Add(item);

            Image icon = item.transform.Find("Icon")?.GetComponent<Image>();
            TextMeshProUGUI label = item.transform.Find("Amount")?.GetComponent<TextMeshProUGUI>();

            if (icon != null && reward.Icon != null) icon.sprite = reward.Icon;
            if (label != null) label.SetText($"x{reward.Amount}");
        }
    }

    private void ClearRewards()
    {
        foreach (var item in _spawnedRewardItems) Destroy(item);
        _spawnedRewardItems.Clear();
    }

    private void OnProceedClicked()
    {
        if (_currentLevel == null) return;
        GameManager.Instance.SetLevelData(_currentLevel);
        SceneManager.LoadScene(_currentLevel.Scene);
    }
}