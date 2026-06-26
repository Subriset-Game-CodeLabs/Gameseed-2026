public class FiniteStateMachine
{
    public IState CurrentState { get; private set; }
 
    public void ChangeState(IState newState)
    {
        CurrentState?.OnExit();
        CurrentState = newState;
        CurrentState?.OnEnter();
    }
 
    public void Update()
    {
        CurrentState?.OnUpdate();
    }
}