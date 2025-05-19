using _StoryGame.Infrastructure.Bootstrap;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace _StoryGame.Gameplay.UI.Impls.Gameplay
{
    public interface IGameplayUIViewModel : IUIViewModel
    {
        public Observable<float> FPS { get; }
        public ReactiveProperty<Vector3> MoveDirection { get; }
        public ReactiveProperty<bool> IsTouchPositionVisible { get; }
        public ReactiveProperty<Vector2> RingPosition { get; }
        void OnOutEvent(PointerOutEvent evt);
        void OnDownEvent(PointerDownEvent evt);
        void OnMoveEvent(PointerMoveEvent evt);
        void OnUpEvent(PointerUpEvent evt);
    }
}
