using UnityEngine;

public class EnemyTurnState : IState
{
    private BattleManager _battleManager;
    private BattleUIManager _battleUIManager;
    private AimController _enemyAimController;
    private Shockwave _enemyShockwave;
    private Stick _enemyStick;

    public EnemyTurnState(BattleManager battleManager, BattleUIManager battleUIManager, AimController aimController, Shockwave shockwave, Stick stick)
    {
        _battleManager = battleManager;
        _enemyAimController = aimController;
        _enemyShockwave = shockwave;
        _enemyStick = stick;
        _battleUIManager = battleUIManager;
    }

    public void OnEnter()
    {
        _battleUIManager.PlayFadeSequence("enemy's turn", () =>
        {
            _enemyAimController.OnAimPositionUpdated += OnAimPositionUpdated;
            _enemyStick.OnLanded += OnLanded;
            _enemyStick.OnLandedOnPlayer += OnLandedOnPlayer;
            _enemyAimController.SetAimDirection(_enemyAimController.GetEdgePoints(3)[1]);
            _enemyAimController.StartAiming();
        });

    }

    private void OnLandedOnPlayer(GameObject playerObject)
    {
        HealthComponent healthComponent = playerObject.GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            EndTurn(healthComponent, 1);
        }
    }

    private void OnAimPositionUpdated()
    {
        Debug.Log("Enemy Stop Aiming");
        _enemyShockwave.Explode(Random.Range(0.2f, 1f), "Enemy");
        Debug.Log("Enemy Smack");
        _enemyStick.StartFlying();
    }

    private void OnLanded(GameObject @object)
    {
        EndTurn();
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

    public void EndTurn(HealthComponent targetHealth = null, int damageAmount = 0)
    {
        _battleManager.ResolveTurnState.SetDamageInfo(true, targetHealth, damageAmount);
        _battleManager.BattleStateMachine.ChangeState(_battleManager.ResolveTurnState);
    }
}