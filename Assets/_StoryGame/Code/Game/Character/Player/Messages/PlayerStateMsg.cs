using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Character.Player.Interfaces;

namespace _StoryGame.Game.Character.Player.Messages
{
    public record PlayerStateMsg(ECharacterState State) : IPlayerMsg
    {
        public string Name => nameof(PlayerStateMsg);
        public ECharacterState State { get; } = State;
    }
}
