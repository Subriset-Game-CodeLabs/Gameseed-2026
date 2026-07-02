using System;
using UnityEngine;

public enum BattleResult
{
    PlayerWin,
    PlayerLose,
    Playing
}

public class BattleManager : MonoBehaviour
{
    private Character _playerCharacter;
    private Character _enemyCharacter;

    [SerializeField]
    private BattleUIManager _battleUIManager;

    [SerializeField]
    private Shockwave _playerHands;

    [SerializeField]
    private Shockwave _enemyHands;

    public FiniteStateMachine BattleStateMachine { get; private set; }
    public PlayerTurnState PlayerTurnState { get; private set; }
    public EnemyTurnState EnemyTurnState { get; private set; }
    public ResolveTurnState ResolveTurnState { get; private set; }

    [SerializeField]
    private SpawnManager _spawnManager;

    //temp
    [SerializeField] private GameObject _enemyPrefab;

    public Character PlayerCharacter => _playerCharacter;
    public Character EnemyCharacter => _enemyCharacter;

    private BattleResult _battleResult;

    void Start()
    {
        StartGame();
    }

    private void Update()
    {
        BattleStateMachine.Update();
    }

    public void UpdateBattleResult(BattleResult result)
    {
        _battleResult = result;
        ShowGameResult();
    }

    public void ShowGameResult()
    {
        switch (_battleResult)
        {
            case BattleResult.PlayerWin:
                _battleUIManager.ShowWinPanel();
                GameManager.Instance.PlayerWin();
                break;
            case BattleResult.PlayerLose:
                _battleUIManager.ShowLosePanel();
                break;
            default:
                break;
        }

    }

    public void StartGame()
    {
        var playerPrefab = GameManager.Instance.PlayerCharacter.BattleInventory.selectedStick.stickPrefab;
        var enemyPrefab = GameManager.Instance.EnemyCharacter.BattleInventory.selectedStick.stickPrefab;

        var (player, enemy) = _spawnManager.SpawnUnit(playerPrefab, enemyPrefab);

        _playerCharacter = player.GetComponent<Character>();
        _enemyCharacter = enemy.GetComponent<Character>();

        _playerCharacter.SetupComponents(GameManager.Instance.PlayerCharacter, _enemyCharacter, _playerHands);
        _enemyCharacter.SetupComponents(GameManager.Instance.EnemyCharacter, _playerCharacter, _enemyHands);

        _battleUIManager.SetupSkillsUI(_playerCharacter.SkillComponent.CharacterSkill, _enemyCharacter.SkillComponent.CharacterSkill);

        _battleUIManager.SetupItemUI();

        _playerCharacter.SkillComponent.OnSkillUsedSuccess += OnPlayerSkillUsedSuccess;
        _playerCharacter.SkillComponent.OnEnergyIncreased += OnEnergyIncreased;

        int playerGoesFirst = PlayerPrefs.GetInt("PlayerGoesFirst");

        BattleStateMachine = new FiniteStateMachine();
        ResolveTurnState = new ResolveTurnState(this, _battleUIManager);
        PlayerTurnState = new PlayerTurnState(this, _battleUIManager, _playerCharacter.AimController, _playerCharacter.CharacterHand, _playerCharacter.Stick, _playerCharacter.SkillComponent);
        EnemyTurnState = new EnemyTurnState(this, _battleUIManager, _enemyCharacter.AimController, _enemyCharacter.CharacterHand, _enemyCharacter.Stick);

        if (playerGoesFirst == 1)
        {
            BattleStateMachine.ChangeState(PlayerTurnState);
        }
        else
        {
            BattleStateMachine.ChangeState(EnemyTurnState);
        }
    }

    private void OnEnergyIncreased(int value)
    {
        _battleUIManager.PlayerRecoverEnergy(value);
    }

    private void OnPlayerSkillUsedSuccess(SkillInstance skill)
    {
        _battleUIManager.PlayFadeSequence("Player use " + skill.Data.name + " skill");
        _battleUIManager.PlayerUseEnergy(skill.Data.manaCost);
        _battleUIManager.UpdateSkillUICooldown(skill, true);
    }

    public void UpdateCooldownUIPlayer()
    {
        foreach (var item in _playerCharacter.SkillComponent.CharacterSkill)
        {
            _battleUIManager.UpdateSkillUICooldown(item, true);
        }
    }

    public void UpdateCooldownUIEnemy()
    {
        foreach (var item in _enemyCharacter.SkillComponent.CharacterSkill)
        {
            _battleUIManager.UpdateSkillUICooldown(item, false);
        }
    }

    public void ResetPosition(Action OnResetFinished = null)
    {
        _battleUIManager.FadeToBlackAndBack(() =>
        {
            _playerCharacter.ResetPosition();
            _enemyCharacter.ResetPosition();
        }, OnResetFinished);

    }
}
