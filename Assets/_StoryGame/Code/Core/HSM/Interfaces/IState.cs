namespace _StoryGame.Core.HSM.Interfaces
{
    public interface IState
    {
        EGameStateType StateType { get; }
        void Enter(IState previousState);
        void Exit(IState previousState);
        void Update();
        IState HandleTransition();
    }
}
