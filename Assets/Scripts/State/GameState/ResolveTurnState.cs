using System;
using UnityEngine;

public class ResolveTurnState : IState
{
    private GameManager _gameManager;
    private HealthComponent _targetHealth;
    private int _damageAmount;
    private bool _isPlayerTakingDamage;
    private bool _waitingForStickToStop;

    public ResolveTurnState(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void SetDamageInfo(HealthComponent targetHealth, int damageAmount, bool isPlayerTakingDamage)
    {
        _targetHealth = targetHealth;
        _damageAmount = damageAmount;
        _isPlayerTakingDamage = isPlayerTakingDamage;
    }

    public void OnEnter()
    {
        if (_targetHealth != null)
        {
            // Apply damage and do something when someone is getting damage
            Debug.Log($"Resolving turn - {(_isPlayerTakingDamage ? "Player" : "Enemy")} taking {_damageAmount} damage");
            _targetHealth.TakeDamage(_damageAmount);
            
            // Update UI to reflect damage taken
            if (_isPlayerTakingDamage)
            {
                UIManager.Instance.PlayerTakeDamage(_damageAmount);
            }
            else
            {
                UIManager.Instance.EnemyTakeDamage(_damageAmount);
            }
            
            // Check win conditions
            if (CheckWinCondition())
            {
                return; // Game ended, don't create shockwave
            }

            // Apply explosion force to both sticks
            ApplyExplosionForceToSticks();
            _waitingForStickToStop = true;
        }
    }

    private void ApplyExplosionForceToSticks()
    {
        Vector3 explosionPosition = Vector3.zero; // Center of the arena
        float explosionForce = 200f;
        float explosionRadius = 5f;
        
        // Apply explosion force to both sticks
        _gameManager.PlayerStick.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
        _gameManager.EnemyStick.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
        
        Debug.Log("Explosion force applied to both sticks!");
    }

    private bool CheckWinCondition()
    {
        HealthComponent playerHealth = _gameManager.PlayerStick.GetComponent<HealthComponent>();
        HealthComponent enemyHealth = _gameManager.EnemyStick.GetComponent<HealthComponent>();

        if (playerHealth != null && enemyHealth != null)
        {
            if (!playerHealth.IsAlive())
            {
                Debug.Log("Player is defeated - Enemy wins!");
                _gameManager.UpdateGameState(GameState.PlayerLose);
                return true; // Game ended
            }
            else if (!enemyHealth.IsAlive())
            {
                Debug.Log("Enemy is defeated - Player wins!");
                _gameManager.UpdateGameState(GameState.PlayerWin);
                return true; // Game ended
            }
        }
        
        return false; // Game continues
    }

    public void OnUpdate()
    {
        if (!_waitingForStickToStop)
            return;

        Stick playerStick = _gameManager.PlayerStick;
        Stick enemyStick = _gameManager.EnemyStick;

        // Check if both sticks have stopped moving
        if (!playerStick.IsFlying && !enemyStick.IsFlying)
        {
            Debug.Log("Both sticks stopped moving - continuing game");
            _waitingForStickToStop = false;
            
            // Transition to the appropriate next state
            if (_isPlayerTakingDamage)
            {
                // Enemy just attacked, now it's player's turn
                _gameManager.GameStateMachine.ChangeState(_gameManager.PlayerTurnState);
            }
            else
            {
                // Player attacked, now it's enemy's turn
                _gameManager.GameStateMachine.ChangeState(_gameManager.EnemyTurnState);
            }
        }
    }

    public void OnExit()
    {
        Debug.Log("Exiting Resolve Turn State");
    }
}
