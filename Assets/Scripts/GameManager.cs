using UnityEngine;

public enum GameState
{
    PlayerWin,
    PlayerLose,
    Playing
}

public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField]
    private AimController _playerAimController;
    [SerializeField]
    private Shockwave _playerShockwave;
    [SerializeField]
    private Stick _playerStick;

    [SerializeField]
    private AimController _enemyAimController;
    [SerializeField]
    private Shockwave _enemyShockwave;
    [SerializeField]
    private Stick _enemyStick;
    
    public FiniteStateMachine GameStateMachine { get; private set; }
    public PlayerTurnState PlayerTurnState { get; private set; }
    public EnemyTurnState EnemyTurnState { get; private set; }
    public ResolveTurnState ResolveTurnState { get; private set; }
    
    public Stick PlayerStick => _playerStick;
    public Stick EnemyStick => _enemyStick;

    private GameState gameState;

    private void Awake()
    {
        GameStateMachine = new FiniteStateMachine();
        ResolveTurnState = new ResolveTurnState(this);
        PlayerTurnState = new PlayerTurnState(this, _playerAimController, _playerShockwave, _playerStick);
        EnemyTurnState = new EnemyTurnState(this, _enemyAimController, _enemyShockwave, _enemyStick);
    }

    void Start()
    {
        GameStateMachine.ChangeState(PlayerTurnState);
    }

    private void Update()
    {
        GameStateMachine.Update();
    }

    public void UpdateGameState(GameState newState)
    {
        gameState = newState;
    }

}