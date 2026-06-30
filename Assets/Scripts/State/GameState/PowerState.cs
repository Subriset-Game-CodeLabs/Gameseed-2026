using System;
using Input;
using UnityEngine;

public class PowerState : IState
{
    private readonly PlayerTurnState _turn;
    private Shockwave _shockwave;
    private Stick _playerStick;
    private BattleUIManager _battleUIManager;

    public PowerState(PlayerTurnState turn, BattleUIManager battleUIManager, Shockwave shockwave, Stick playerStick)
    {
        _turn = turn;
        _shockwave = shockwave;
        _playerStick = playerStick;
        _battleUIManager = battleUIManager;
    }

    public void OnEnter()
    {
        Debug.Log("Entering Power State");
        _playerStick.OnLanded += OnLanded;
        _playerStick.OnLandedOnEnemy += OnLandedOnEnemy;
        _battleUIManager.ShowPowerUI();
        _battleUIManager.OnSmackPressed += OnSmackPressed;
    }

    private void OnSmackPressed(float powerModifier)
    {
        _battleUIManager.HidePowerUI();
        _shockwave.Explode(powerModifier, "Player");
        _playerStick.StartFlying();
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {
        _playerStick.OnLanded -= OnLanded;
        _playerStick.OnLandedOnEnemy -= OnLandedOnEnemy;
        _battleUIManager.OnSmackPressed -= OnSmackPressed;
        Debug.Log("Exiting Power State");
    }

    private void OnLandedOnEnemy(GameObject enemyObject)
    {
        HealthComponent healthComponent = enemyObject.GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            _turn.EndTurn(healthComponent, 1);
        }
    }

    private void OnLanded(GameObject @object)
    {
        _turn.EndTurn();
    }
}