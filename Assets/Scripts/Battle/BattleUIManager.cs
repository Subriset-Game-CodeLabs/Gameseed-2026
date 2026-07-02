using System;
using DG.Tweening;
using Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    // [SerializeField] private Button _smackButton;
    [SerializeField] private Slider _powerSlider;
    [SerializeField] private TMP_Text _powerText;
    [SerializeField] private HealthUI _playerHealthUI;
    [SerializeField] private HealthUI _enemyHealthUI;
    [SerializeField] private EnergyUI _playerEnergyUI;
    [SerializeField] private EnergyUI _enemyEnergyUI;

    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    [SerializeField] private GameObject _rewardPanel;

    [SerializeField] private SkillUI _playerSkillUI;
    [SerializeField] private SkillUI _enemySkillUI;

    [SerializeField] private ItemUI _playerItemUI;
    [SerializeField] private Button _itemButton;

    [SerializeField]
    private float _sliderSpeed = 1f;
    private bool _isPowering;

    [SerializeField] private TMP_Text _announcerText;
    [SerializeField] private Image _blackScreen;


    [Header("Settings for Announcer Text")]
    public float fadeDuration = 1f;
    public float holdDuration = 1f; // time to stay visible between fade in and fade out
    public Ease easeType = Ease.InOutSine;

    private Tween _blinkTween;

    public event Action<float> OnSmackPressed;
    public event Action<SkillInstance> OnSkillPressed;
    public event Action<BaseItem> OnItemPressed;

    public void ShowPowerUI()
    {
        // _smackButton.gameObject.SetActive(true);
        _powerSlider.gameObject.SetActive(true);
        _isPowering = true;
        _powerSlider.value = 0;
        StartBlinking();
    }

    void Update()
    {
        if (_isPowering)
        {
            _powerSlider.value = Mathf.PingPong(Time.time * _sliderSpeed, 1f);
        }
    }

    public void StartBlinking()
    {
        StopBlinking();

        _blinkTween = _powerText
            .DOFade(0f, 0.5f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }

    public void StopBlinking()
    {
        _blinkTween?.Kill();
        _blinkTween = null;

        if (_powerText != null)
        {
            var c = _powerText.color;
            c.a = 1f;
            _powerText.color = c;
        }
    }

    public void SetupSkillsUI(SkillInstance[] playerSkills, SkillInstance[] enemySkills)
    {
        _playerSkillUI.SetupUI(playerSkills, true);
        _playerSkillUI.OnSkillPressed += OnSkillPressedUI;
        _enemySkillUI.SetupUI(enemySkills);
    }

    public void UpdateSkillUICooldown(SkillInstance skillInstance, bool isPlayer)
    {
        if (isPlayer)
        {
            _playerSkillUI.UpdateUI(skillInstance);
        }
        else
        {
            _enemySkillUI.UpdateUI(skillInstance);
        }
    }

    public void SetupItemUI()
    {
        _playerItemUI.SetupUI();
        _playerItemUI.OnItemPressed += OnItemPressedUI;
        _itemButton.onClick.AddListener(() =>
        {
            if (_playerItemUI.gameObject.activeSelf)
            {
                _playerItemUI.Hide();
            }
            else
            {
                _playerItemUI.Show();
            }
        });
    }

    public void SetupHealthUI(HealthComponent playerHealth, HealthComponent enemyHealth)
    {
        _playerHealthUI.Initialize(playerHealth);
        _enemyHealthUI.Initialize(enemyHealth);
    }

    private void OnItemPressedUI(BaseItem item)
    {
        OnItemPressed?.Invoke(item);
        _playerItemUI.RefreshUI();
    }

    private void OnSkillPressedUI(SkillInstance skill)
    {
        OnSkillPressed?.Invoke(skill);
    }

    private void Start()
    {
        InputManager.Instance.PlayerInput.Smash.OnDown += OnSmash;
    }
    private void OnSmash()
    {
        OnSmackPressed?.Invoke(_powerSlider.value);
        HidePowerUI();
    }

    void OnDisable()
    {
        InputManager.Instance.PlayerInput.Smash.OnDown -= OnSmash;
    }

    public void HidePowerUI()
    {
        _isPowering = false;
        // _smackButton.gameObject.SetActive(false);
        _powerSlider.gameObject.SetActive(false);
        StopBlinking();
    }



    public void SetupEnergyUI(SkillComponent playerSkillComponent, SkillComponent enemySkillComponent)
    {
        _playerEnergyUI.Initialize(playerSkillComponent);
        _enemyEnergyUI.Initialize(enemySkillComponent);
    }

    public void PlayFadeSequence(string text, Action onComplete = null)
    {
        _announcerText.text = text;
        Sequence fadeSequence = DOTween.Sequence();

        fadeSequence.Append(
            _announcerText.DOFade(1f, fadeDuration).SetEase(easeType)
        );

        fadeSequence.AppendInterval(holdDuration);

        fadeSequence.Append(
            _announcerText.DOFade(0f, fadeDuration).SetEase(easeType)
        );

        fadeSequence.OnComplete(() => onComplete?.Invoke());
    }

    public void FadeToBlackAndBack(Action onScreenBlack = null, Action onComplete = null)
    {
        Sequence fadeSequence = DOTween.Sequence();

        fadeSequence.Append(
            _blackScreen.DOFade(1f, 0.5f).SetEase(easeType)
        );

        fadeSequence.AppendCallback(() => onScreenBlack?.Invoke());

        fadeSequence.Append(
            _blackScreen.DOFade(0f, 1).SetEase(easeType)
        );

        fadeSequence.OnComplete(() => onComplete?.Invoke());

    }

    public void ShowWinPanel() => _winPanel.SetActive(true);
    public void HideWinPanel() => _winPanel.SetActive(false);
    public void ShowLosePanel() => _losePanel.SetActive(true);
    public void HideLosePanel() => _losePanel.SetActive(false);
}