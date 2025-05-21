using UnityEngine;

namespace _StoryGame.Game.Movement
{
    public record DestinationPointMsg(Vector3 Position) : IMovementProcessorMsg
    {
        public string Name => nameof(DestinationPointMsg);
        public Vector3 Position { get; } = Position;
    }
}
