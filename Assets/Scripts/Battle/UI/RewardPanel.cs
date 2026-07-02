using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardPanel : MonoBehaviour
{
    [SerializeField]
    private Button _nextLevelButton;
    [SerializeField]
    private Button _mainMenuButton;
    [SerializeField]
    private string _nextLevelScene;
    [SerializeField]
    private string _mainMenuScene;
    [SerializeField] 
    private GameObject _rewardItemPrefab;
    [SerializeField] 
    private Transform _rewardItemParent;
    private readonly List<GameObject> _spawnedRewardItems = new List<GameObject>();

    void Start()
    {
        _nextLevelButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(_nextLevelScene);    
        });

        _mainMenuButton.onClick.AddListener(() =>
        {
           SceneManager.LoadScene(_mainMenuScene); 
        });
    }

    public void OnEnable()
    {
        PopulateRewards(GameManager.Instance.LevelData.Rewards);
    }

    public void OnDisable()
    {
        gameObject.SetActive(false);
    }

    private void PopulateRewards(List<RewardData> rewards)
    {
        ClearRewards();
        if (rewards == null) return;

        foreach (var reward in rewards)
        {
            GameObject item = Instantiate(_rewardItemPrefab, transform);
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
}
