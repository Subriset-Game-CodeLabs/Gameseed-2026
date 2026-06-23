using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : PersistentSingleton<UIManager>
{
    [SerializeField] private Button _smackButton;
    [SerializeField] private Slider _powerSlider;
    [SerializeField] private HealthUI playerHealthUI;
    [SerializeField] private HealthUI enemyHealthUI;

    private GameObject _playerHealths;
    private GameObject _enemyHealths;

    public event Action<float> OnSmackPressed;

    public void ShowPowerUI()
    {
        _smackButton.gameObject.SetActive(true);
        _powerSlider.gameObject.SetActive(true);
    }

    void Start()
    {
        _smackButton.onClick.AddListener(() =>
        {
            OnSmackPressed?.Invoke(_powerSlider.value);
        });
        HidePowerUI();
    }

    public void HidePowerUI()
    {
        _smackButton.gameObject.SetActive(false);
        _powerSlider.gameObject.SetActive(false);
    }

    public void PlayerTakeDamage(int damage)
    {
        if (playerHealthUI != null)
        {
            playerHealthUI.TakeDamage(damage);
        }
    }

    public void EnemyTakeDamage(int damage)
    {
        if (enemyHealthUI != null)
        {
            enemyHealthUI.TakeDamage(damage);
        }
    }
}