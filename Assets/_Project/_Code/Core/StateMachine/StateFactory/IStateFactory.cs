namespace TankPlayground.Core.StateMachine
{
    public interface IStateFactory 
    {
        IState Create<TState>() where TState : IState;
    }
}