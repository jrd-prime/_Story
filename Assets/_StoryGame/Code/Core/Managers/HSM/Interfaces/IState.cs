namespace _StoryGame.Core.Managers.HSM.Interfaces
{
    public interface IState
    {
        void Enter(IState previousState);
        void Exit(IState previousState);
        void Update();
        IState HandleTransition();
    }
}
