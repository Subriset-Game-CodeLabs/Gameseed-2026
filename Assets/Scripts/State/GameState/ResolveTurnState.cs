using System;
using UnityEngine;

public class ResolveTurnState : IState
{
    private BattleManager _battleManager;
    private BattleUIManager _battleUiManager;
    private HealthComponent _targetHealth;
    private int _damageAmount;
    private bool _isEnemyTurn;
    private int _currentTurn = 1;

    public ResolveTurnState(BattleManager battleManager, BattleUIManager battleUIManager)
    {
        _battleManager = battleManager;
        _battleUiManager = battleUIManager;
    }

    public void SetDamageInfo(bool isEnemyTurn, HealthComponent targetHealth, int damageAmount)
    {
        _targetHealth = targetHealth;
        _damageAmount = damageAmount;
        _isEnemyTurn = isEnemyTurn;
    }

    public void OnEnter()
    {
        _currentTurn += 1;

        if (_targetHealth != null)
        {
            _targetHealth.TakeDamage(_damageAmount);

            if (CheckWinCondition())
            {
                return;
            }

            _battleManager.ResetPosition(() =>
           {
               HandleNextTurn();
           });
        }
        else
        {
            HandleNextTurn();
        }


    }

    private void HandleNextTurn()
    {
        if (_isEnemyTurn)
        {
            if (_battleManager.PlayerCharacter.IsSkipped)
            {
                _currentTurn += 1;
                _battleManager.BattleStateMachine.ChangeState(_battleManager.EnemyTurnState);
            }
            else
            {
                _battleManager.BattleStateMachine.ChangeState(_battleManager.PlayerTurnState);
            }
        }
        else
        {
            if (_battleManager.EnemyCharacter.IsSkipped)
            {
                _currentTurn += 1;
                _battleManager.BattleStateMachine.ChangeState(_battleManager.PlayerTurnState);
            }
            else
            {
                _battleManager.BattleStateMachine.ChangeState(_battleManager.EnemyTurnState);
            }
        }

    }

    private bool CheckWinCondition()
    {
        HealthComponent playerHealth = _battleManager.PlayerCharacter.HealthComponent;
        HealthComponent enemyHealth = _battleManager.EnemyCharacter.HealthComponent;

        if (playerHealth != null && enemyHealth != null)
        {
            if (!playerHealth.IsAlive())
            {
                Debug.Log("Player is defeated - Enemy wins!");
                _battleManager.UpdateBattleResult(BattleResult.PlayerLose);
                return true; // Game ended
            }
            else if (!enemyHealth.IsAlive())
            {
                Debug.Log("Enemy is defeated - Player wins!");
                _battleManager.UpdateBattleResult(BattleResult.PlayerWin);
                return true; // Game ended
            }
        }

        return false; // Game continues
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {
        _targetHealth = null;
        Debug.Log(_currentTurn);

        if (_currentTurn % 2 == 0)
        {
            _battleManager.PlayerCharacter.SkillComponent.TickCooldowns();
            _battleManager.EnemyCharacter.SkillComponent.TickCooldowns();

            _battleManager.UpdateCooldownUIPlayer();
            _battleManager.UpdateCooldownUIEnemy();

            _battleManager.PlayerCharacter.SkillComponent.AddEnergy(1);
            _battleManager.EnemyCharacter.SkillComponent.AddEnergy(1);

            _battleManager.PlayerCharacter.ResetStats();
            _battleManager.EnemyCharacter.ResetStats();
        }

        _battleManager.PlayerCharacter.ClearStatus();
        _battleManager.EnemyCharacter.ClearStatus();



        Debug.Log("Exiting Resolve Turn State");
    }
}
