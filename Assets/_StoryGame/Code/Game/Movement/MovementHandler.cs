using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer.Unity;

namespace _StoryGame.Game.Movement
{
    public sealed class MovementHandler : IMovementHandler, IInitializable
    {
        public ReadOnlyReactiveProperty<Vector3> DestinationPoint { get; } 
        public ReadOnlyReactiveProperty<Vector3> MoveDirection => _moveDirection;
        public ReadOnlyReactiveProperty<bool> IsTouchVisible => _isTouchVisible;
        public ReadOnlyReactiveProperty<Vector2> RingPosition => _ringPosition;

        private const float OffsetForFullSpeed = 150f;

        private bool _isTouchActive;
        private Vector3 _moveInput;
        private Vector3 _startTouchPosition;

        private readonly ReactiveProperty<Vector3> _moveDirection = new(Vector3.zero);
        private readonly ReactiveProperty<bool> _isTouchVisible = new(false);
        private readonly ReactiveProperty<Vector2> _ringPosition = new(Vector2.zero);

        public void Initialize()
        {
        }

        private void SetMoveDirection(Vector3 value) => _moveDirection.Value = value;

        public void OnPointerDown(PointerDownEvent evt)
        {
            if (_isTouchActive) return;

            _isTouchActive = true;
            _startTouchPosition = evt.localPosition;

            ShowRingAtTouchPosition(_startTouchPosition);
        }

        public void OnPointerMove(PointerMoveEvent evt)
        {
            if (!_isTouchActive) return;

            var currentPosition = evt.localPosition;
            var offset = currentPosition - _startTouchPosition;
            var distance = offset.magnitude;

            switch (distance)
            {
                case < OffsetForFullSpeed * .2f:
                    SetMoveDirection(Vector3.zero);
                    return;
                case > OffsetForFullSpeed:
                    offset = offset.normalized * OffsetForFullSpeed;
                    break;
            }

            _moveInput = offset / OffsetForFullSpeed;
            _moveInput = Vector2.ClampMagnitude(_moveInput, 1.0f);

            SetMoveDirection(new Vector3(_moveInput.x, 0, _moveInput.y * -1f));
        }

        public void OnPointerUp(PointerUpEvent evt)
        {
            if (!_isTouchActive) return;
            ResetTouch();
        }

        public void OnPointerCancel(PointerOutEvent evt)
        {
            if (!_isTouchActive) return;
            ResetTouch();
        }

        private void ResetTouch()
        {
            _isTouchActive = false;
            _moveInput = Vector2.zero;

            SetMoveDirection(Vector3.zero);
            HideRing();
        }

        private void ShowRingAtTouchPosition(Vector3 position)
        {
            _ringPosition.Value = new Vector2(position.x, position.y);

            ShowRing();
        }

        private void HideRing() => _isTouchVisible.Value = false;
        private void ShowRing() => _isTouchVisible.Value = true;
    }
}
