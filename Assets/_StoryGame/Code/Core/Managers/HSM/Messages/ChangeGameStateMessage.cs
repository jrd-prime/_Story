using _StoryGame.Core.Managers.HSM.Impls.States;
using _StoryGame.Core.Managers.HSM.Interfaces;

namespace _StoryGame.Core.Managers.HSM.Messages
{
    public record ChangeGameStateMessage(GameStateType StateType) : IHSMMessage
    {
        public string Name => "Change Game State";
        public GameStateType StateType { get; } = StateType;
    }
}
