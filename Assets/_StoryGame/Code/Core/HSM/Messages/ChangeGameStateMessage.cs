using _StoryGame.Core.HSM.Impls.States;
using _StoryGame.Core.HSM.Interfaces;

namespace _StoryGame.Core.HSM.Messages
{
    public record ChangeGameStateMessage(GameStateType StateType) : IHSMMessage
    {
        public string Name => "Change Game State";
        public GameStateType StateType { get; } = StateType;
    }
}
