using _StoryGame.Core.HSM.Impls.States;

namespace _StoryGame.Core.HSM.Interfaces
{
    public interface IState
    {
        GameStateType StateType { get; }
        void Enter(IState previousState);
        void Exit(IState previousState);
        void Update();
        IState HandleTransition();
    }
}
