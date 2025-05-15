using _StoryGame.Data;
using ModestTree;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace _StoryGame
{
    public class FullScreenMovementViewModel : IInitializable
    {
        public ReactiveProperty<Vector3> MoveDirection { get; } = new(Vector3.zero);
        public ReactiveProperty<bool> IsTouchPositionVisible { get; } = new(false);
        public ReactiveProperty<Vector2> RingPosition { get; } = new(Vector2.zero);

        private bool _isTouchActive;
        private float _offsetForFullSpeed = 100f;
        private Vector3 _moveInput;
        private Vector3 _startTouchPosition;

        private ISettingsManager _settingsManager;

        public void Initialize()
        {
            // if (_settingsManager != null)
            // {
            //     var movementControlSettings = _settingsManager.GetConfig<MovementControlSettings>();
            //     // _offsetForFullSpeed = movementControlSettings.offsetForFullSpeed;
            //     return;
            // }
            //
            // Debug.LogError("SettingsManager is null. Use default settings.");
        }

        private void SetMoveDirection(Vector3 value) => MoveDirection.Value = value;


        public void OnDownEvent(PointerDownEvent evt)
        {
            Log.Warn(" OnDownEvent");
            if (_isTouchActive) return;

            _isTouchActive = true;
            _startTouchPosition = evt.localPosition;

            ShowRingAtTouchPosition(_startTouchPosition);
        }

        public void OnMoveEvent(PointerMoveEvent evt)
        {
            if (!_isTouchActive) return;
            Log.Warn("OnMoveEvent");
            var currentPosition = evt.localPosition;
            var offset = currentPosition - _startTouchPosition;
            var distance = offset.magnitude;

            if (distance > _offsetForFullSpeed) offset = offset.normalized * _offsetForFullSpeed;

            _moveInput = offset / _offsetForFullSpeed;
            _moveInput = Vector2.ClampMagnitude(_moveInput, 1.0f);

            Log.Warn(_moveInput.ToString());

            SetMoveDirection(new Vector3(_moveInput.x, 0, _moveInput.y * -1f));
        }

        public void OnUpEvent(PointerUpEvent _)
        {
            if (!_isTouchActive) return;

            Log.Warn(" OnUpEvent");
            ResetTouch();
        }

        public void OnOutEvent(PointerOutEvent _)
        {
            if (!_isTouchActive) return;

            Log.Warn(" OnOutEvent");
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
            RingPosition.Value = new Vector2(position.x, position.y);

            ShowRing();
        }

        private void HideRing() => IsTouchPositionVisible.Value = false;
        private void ShowRing() => IsTouchPositionVisible.Value = true;
    }

    public class MovementControlSettings : SettingsBase
    {
        public float offsetForFullSpeed;
        public override string ConfigName { get; }
    }

    internal interface ISettingsManager
    {
        T GetConfig<T>();
    }
}
