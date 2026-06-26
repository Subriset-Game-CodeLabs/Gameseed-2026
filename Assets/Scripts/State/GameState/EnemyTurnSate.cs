using System;
using UnityEngine;

public class EnemyTurnState : IState
{
    private GameManager _gameManager;
    private AimController _enemyAimController;
    private Shockwave _enemyShockwave;
    private Stick _enemyStick;

    public EnemyTurnState(GameManager gameManager, AimController aimController, Shockwave shockwave, Stick stick)
    {
        _gameManager = gameManager;
        _enemyAimController = aimController;
        _enemyShockwave = shockwave;
        _enemyStick = stick;
    }

    public void OnEnter()
    {
        _enemyAimController.OnAimPositionUpdated += OnAimPositionUpdated;
        _enemyStick.OnLanded += OnLanded;
        _enemyStick.OnLandedOnPlayer += OnLandedOnPlayer;
        _enemyAimController.SetUseMouseInput(false);
        _enemyAimController.SetAimDirection(_enemyAimController.GetEdgePoints(3)[1]);
        _enemyAimController.StartAiming();
    }

    private void OnLandedOnPlayer(GameObject playerObject)
    {
        HealthComponent healthComponent = playerObject.GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            // Transition to ResolveTurnState with damage info
            _gameManager.ResolveTurnState.SetDamageInfo(healthComponent, 1, true);
            _gameManager.GameStateMachine.ChangeState(_gameManager.ResolveTurnState);
        }
    }

    private void OnAimPositionUpdated()
    {
        Debug.Log("Enemy Stop Aiming");
        _enemyShockwave.Explode(1, "Enemy");
        Debug.Log("Enemy Smack");
        _enemyStick.StartFlying();
    }

    private void OnLanded(GameObject @object)
    {
        _gameManager.GameStateMachine.ChangeState(_gameManager.PlayerTurnState);
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {
        Debug.Log("Exiting Enemy's Turn");
        _enemyAimController.OnAimPositionUpdated -= OnAimPositionUpdated;
        _enemyStick.OnLanded -= OnLanded;
        _enemyStick.OnLandedOnPlayer -= OnLandedOnPlayer;
    }
}