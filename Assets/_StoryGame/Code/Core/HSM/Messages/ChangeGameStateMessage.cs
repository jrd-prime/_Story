using _StoryGame.Core.HSM.Impls.States;
using _StoryGame.Core.HSM.Interfaces;

namespace _StoryGame.Core.HSM.Messages
{
    public record ChangeGameStateMessage(EGameStateType StateType) : IHSMMessage
    {
        public string Name => "Change Game State";
        public EGameStateType StateType { get; } = StateType;
    }
}
