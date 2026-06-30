using System;
using UnityEngine;

public class AimState : IState
{
    private readonly PlayerTurnState _turn;
    private  AimController _aimController;

    public AimState(PlayerTurnState turn, AimController aimController)
    {
        _turn = turn;
        _aimController = aimController;
    }

    public void OnEnter()
    {
        Debug.Log("Entering Aiming State");
        Debug.Log(_aimController);
        _aimController.StartAiming();
        _aimController.OnStopAiming += OnStopAiming;
    }

    private void OnStopAiming()
    {
        _turn.GoTo(_turn.PowerState);
    }

    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        _aimController.OnStopAiming -= OnStopAiming;
        Debug.Log("Exiting Aiming State");
    }
}