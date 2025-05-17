using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _StoryGame.Infrastructure.Input
{
    public sealed class MobileInput : MonoBehaviour, IJInput
    {
        public Observable<Vector2> TouchBegan => _touchBeganSubject.AsObservable();
        public Observable<Vector2> TouchMoved => _touchMovedSubject.AsObservable();
        public Observable<Vector2> TouchEnded => _touchEndedSubject.AsObservable();

        private readonly Subject<Vector2> _touchBeganSubject = new();
        private readonly Subject<Vector2> _touchMovedSubject = new();
        private readonly Subject<Vector2> _touchEndedSubject = new();

        private InputAction _position;
        private InputAction _contact;
        private JInputActions _gameInputActions;

        private void Awake()
        {
            _gameInputActions = new JInputActions();
            _gameInputActions.Enable();

            _position = _gameInputActions.UI.Point;
            _contact = _gameInputActions.UI.Click;

            _contact.started += HandleStart;
            _position.performed += HandleMove;
            _contact.canceled += HandleEnd;
        }

        private void HandleStart(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started) return;

            var touchPos = _position.ReadValue<Vector2>();
            _touchBeganSubject.OnNext(touchPos);
        }

        private void HandleMove(InputAction.CallbackContext context)
        {
            var touchPos = _position.ReadValue<Vector2>();

            if (touchPos == Vector2.zero)
                return;

            _touchMovedSubject.OnNext(touchPos);
        }

        private void HandleEnd(InputAction.CallbackContext context)
        {
            var touchPos = _position.ReadValue<Vector2>();
            _touchEndedSubject.OnNext(touchPos);
        }

        private void OnDestroy()
        {
            _contact.started -= HandleStart;
            _position.performed -= HandleMove;
            _contact.canceled -= HandleEnd;

            _gameInputActions?.Dispose();
        }
    }
}
