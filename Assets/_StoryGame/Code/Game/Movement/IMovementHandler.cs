using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace _StoryGame.Game.Movement
{
    public interface IMovementHandler
    {
        public  ReadOnlyReactiveProperty<Vector3> MoveDirection { get; }
        public ReadOnlyReactiveProperty<bool> IsTouchVisible { get; }
        public ReadOnlyReactiveProperty<Vector2> RingPosition { get; }
        void OnPointerDown(PointerDownEvent evt);
        void OnPointerMove(PointerMoveEvent evt);
        void OnPointerUp(PointerUpEvent evt);
        void OnPointerCancel(PointerOutEvent evt);
    }
}
