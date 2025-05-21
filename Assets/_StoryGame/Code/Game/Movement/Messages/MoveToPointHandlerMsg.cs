using UnityEngine;

namespace _StoryGame.Game.Movement
{
    public record MoveToPointHandlerMsg(Vector3 Position) : IMovementHandlerMsg
    {
        public string Name => nameof(MoveToPointHandlerMsg);
        public Vector3 Position { get; } = Position;
    }
}
