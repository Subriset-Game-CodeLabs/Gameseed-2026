using System;
using UnityEngine;

public class PowerState : IState
{
    private readonly PlayerTurnState _turn;
    private Shockwave _shockwave;
    private Stick _playerStick;

    public PowerState(PlayerTurnState turn, Shockwave shockwave, Stick playerStick)
    {
        _turn = turn;
        _shockwave = shockwave;
        _playerStick = playerStick;
    }

    public void OnEnter()
    {
        Debug.Log("Entering Power State");
        _playerStick.OnLanded += OnLanded;
        _playerStick.OnLandedOnEnemy += OnLandedOnEnemy;
        UIManager.Instance.ShowPowerUI();
        UIManager.Instance.OnSmackPressed += OnSmackPressed;
    }

    private void OnSmackPressed(float powerModifier)
    {
        UIManager.Instance.HidePowerUI();
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
        UIManager.Instance.OnSmackPressed -= OnSmackPressed;
        Debug.Log("Exiting Power State");
    }

    private void OnLandedOnEnemy(GameObject enemyObject)
    {
        HealthComponent healthComponent = enemyObject.GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            _turn.ResolveDamage(healthComponent, 1);
        }
    }

    private void OnLanded(GameObject @object)
    {
        _turn.EndTurn();
    }
}