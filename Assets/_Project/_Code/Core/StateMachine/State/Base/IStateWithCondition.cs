namespace TankPlayground.Core.StateMachine
{
    public interface IStateWithCondition : IState
    {
        bool CanBeEntered();
    }
}