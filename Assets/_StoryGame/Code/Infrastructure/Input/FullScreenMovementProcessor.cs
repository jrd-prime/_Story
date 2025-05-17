using System;
using _StoryGame.Infrastructure.Logging;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace _StoryGame.Infrastructure.Input
{
    public sealed class FullScreenMovementProcessor : IInitializable, IDisposable
    {
        public ReactiveProperty<Vector3> MoveDirection { get; } = new(Vector3.zero);
        public ReactiveProperty<bool> IsTouchPositionVisible { get; } = new(false);
        public ReactiveProperty<Vector2> RingPosition { get; } = new(Vector2.zero);

        private readonly IJLog _log;
        private readonly IJInput _input;

        private bool _isTouchActive;
        private const float OffsetForFullSpeed = 100f;
        private Vector2 _startTouchPosition;
        private readonly CompositeDisposable _disposables = new();

        public FullScreenMovementProcessor(IJInput input, IJLog log) => (_input, _log) = (input, log);

        public void Initialize()
        {
            if (_input == null)
                return;

            _input.TouchBegan.Subscribe(OnTouchBegan).AddTo(_disposables);
            _input.TouchMoved.Subscribe(OnTouchMoved).AddTo(_disposables);
            _input.TouchEnded.Subscribe(OnTouchEnded).AddTo(_disposables);
        }

        private void OnTouchBegan(Vector2 position)
        {
            if (_isTouchActive)
                return;

            // _log.Debug($"OnTouchBegan at {position}");

            _isTouchActive = true;
            _startTouchPosition = position;
            RingPosition.Value = position;
            IsTouchPositionVisible.Value = true;
        }

        private void OnTouchEnded(Vector2 position)
        {
            // _log.Debug($"OnTouchEnded, _isTouchActive: {_isTouchActive}");
            _isTouchActive = false;
            MoveDirection.Value = Vector3.zero;
            IsTouchPositionVisible.Value = false;
        }

        private void OnTouchMoved(Vector2 currentPosition)
        {
            if (!_isTouchActive)
                return;

            // _log.Debug($"OnTouchMoved at {currentPosition}");

            var offset = currentPosition - _startTouchPosition;
            var distance = offset.magnitude;

            if (distance > OffsetForFullSpeed)
                offset = offset.normalized * OffsetForFullSpeed;

            var moveInput = offset / OffsetForFullSpeed;
            MoveDirection.Value = new Vector3(moveInput.x, 0, moveInput.y);
        }

        public void Dispose() => _disposables.Dispose();
    }
}
