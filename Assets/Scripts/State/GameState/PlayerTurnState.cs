using System;
using Input;
using Mono.Cecil;
using UnityEngine;

public class PlayerTurnState : IState
{
    private GameManager _gameManager;
    private readonly FiniteStateMachine _subState = new FiniteStateMachine();

    public AimState AimState {get; private set;}
    public PowerState PowerState {get; private set;}

    public PlayerTurnState(GameManager gameManager, AimController aimController, Shockwave shockwave, Stick playerStick)
    {
        _gameManager = gameManager;
        AimState = new AimState(this, aimController);
        PowerState = new PowerState(this, shockwave, playerStick);
    }


    public void OnEnter()
    {
        Debug.Log("Player's Turn");
        _subState.ChangeState(AimState);
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {
        Debug.Log("Exiting Player's Turn");
    }

    public void GoTo(IState next) => _subState.ChangeState(next);
 
    // ResolveState calls this once the roll's result is fully handled
    public void EndTurn() => _gameManager.GameStateMachine.ChangeState(_gameManager.EnemyTurnState);
    
    public void ResolveDamage(HealthComponent targetHealth, int damageAmount) 
    {
        _gameManager.ResolveTurnState.SetDamageInfo(targetHealth, damageAmount, false);
        _gameManager.GameStateMachine.ChangeState(_gameManager.ResolveTurnState);
    }
}