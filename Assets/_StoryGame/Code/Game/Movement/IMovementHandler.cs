using R3;
using UnityEngine;

namespace _StoryGame.Game.Movement
{
    public interface IMovementHandler
    {
        public ReadOnlyReactiveProperty<Vector3> DestinationPoint { get; }
        public  ReadOnlyReactiveProperty<Vector3> MoveDirection { get; }
    }
}
