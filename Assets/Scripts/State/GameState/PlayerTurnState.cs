using System;
using Input;
using Mono.Cecil;
using UnityEngine;

public class PlayerTurnState : IState
{
    private BattleManager _battleManager;
    private BattleUIManager _battleUIManager;
    private SkillComponent _skillComponent;
    private readonly FiniteStateMachine _subState = new FiniteStateMachine();

    public AimState AimState { get; private set; }
    public PowerState PowerState { get; private set; }

    public PlayerTurnState(BattleManager battleManager, BattleUIManager battleUIManager, AimController aimController, Shockwave shockwave, Stick playerStick, SkillComponent skillComponent)
    {
        _battleManager = battleManager;
        _battleUIManager = battleUIManager;
        _skillComponent = skillComponent;
        AimState = new AimState(this, aimController);
        PowerState = new PowerState(this, _battleUIManager, shockwave, playerStick);
    }


    public void OnEnter()
    {
        _battleUIManager.PlayFadeSequence("player's turn", () =>
        {
            
        });
        _battleUIManager.OnSkillPressed += OnSkillPressed;
        _battleUIManager.OnItemPressed += OnItemPressed;
        InputManager.Instance.PlayerInput.Smash.OnDown += OnSmash;
    }

    private void OnItemPressed(BaseItem item)
    {
        _battleManager.PlayerCharacter.UseItem(item, _battleManager.PlayerCharacter, _battleManager.EnemyCharacter);

    }

    private void OnSkillPressed(SkillInstance skill)
    {
        _battleManager.PlayerCharacter.UseSkill(skill, _battleManager.PlayerCharacter, _battleManager.EnemyCharacter);
    }

    private void OnSmash()
    {
        _subState.ChangeState(AimState);
        InputManager.Instance.PlayerInput.Smash.OnDown -= OnSmash;
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {
        Debug.Log("Exiting Player's Turn");
        _battleUIManager.OnSkillPressed -= OnSkillPressed;
        _battleUIManager.OnItemPressed -= OnItemPressed;
    }

    public void GoTo(IState next) => _subState.ChangeState(next);

    public void EndTurn(HealthComponent targetHealth = null, int damageAmount = 0)
    {
        _battleManager.ResolveTurnState.SetDamageInfo(false, targetHealth, damageAmount);
        _battleManager.BattleStateMachine.ChangeState(_battleManager.ResolveTurnState);
    }
}