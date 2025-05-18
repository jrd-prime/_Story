using System;
using _StoryGame.Core.Managers.HSM.Interfaces;

namespace _StoryGame.Core.Managers.HSM.Messages
{
    public record ChangeGameStateMessage(Type StateType) : IHSMMessage
    {
        public string Name => "Change Game State";
        public Type StateType { get; } = StateType;
    }
}
