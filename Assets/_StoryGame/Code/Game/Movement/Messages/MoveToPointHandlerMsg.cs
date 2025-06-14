using _StoryGame.Core.Interfaces.Publisher.Messages;
using UnityEngine;

namespace _StoryGame.Game.Movement.Messages
{
    public record MoveToPointHandlerMsg(Vector3 Position) : IMovementHandlerMsg
    {
        public string Name => nameof(MoveToPointHandlerMsg);
        public Vector3 Position { get; } = Position;
    }
}
