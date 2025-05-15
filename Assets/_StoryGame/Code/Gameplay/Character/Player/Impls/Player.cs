using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Character.Player.Interfaces;
using ModestTree;
using R3;
using UnityEngine;
using Zenject;

namespace _StoryGame.Gameplay.Character.Player.Impls
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Animator))]
    public sealed class Player : MonoBehaviour, IPlayer
    {
        [SerializeField] private PlayerFrontTriggerArea frontTriggerArea;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float acceleration = 0.1f;

        public ReactiveProperty<Vector3> Position { get; } = new();
        public string Id => _interactor.Id;
        public string Name => _interactor.Name;
        public string Description => _interactor.Description;
        public object Animator { get; private set; }
        public int Health => _interactor.Health;
        public int MaxHealth => _interactor.MaxHealth;
        public CharacterState State { get; private set; } = CharacterState.Idle;

        [Inject] private PlayerInteractor _interactor;

        private Rigidbody _rb;
        private Vector3 _currentVelocity;
        private Camera _mainCamera;
        private Vector3 _previousPosition;

        private void Start()
        {
            if (!frontTriggerArea) throw new NullReferenceException($"{nameof(frontTriggerArea)} is null. {name}");

            frontTriggerArea.Init(this);
            Animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
            _mainCamera = _interactor.MainCamera;
        }

        private void FixedUpdate()
        {
            if (State == CharacterState.Interacting) return;

            if (_interactor.MoveDirection != Vector3.zero) Log.Warn(_interactor.MoveDirection.ToString());
            Move(_interactor.MoveDirection);
            if (_previousPosition != transform.position) UpdatePosition();
        }

        private void UpdatePosition()
        {
            var position = transform.position;
            _previousPosition = position;
            Position.Value = position;
            _interactor.SetPosition(position);
        }

        private void Move(Vector3 moveDirection)
        {
            if (moveDirection != Vector3.zero)
            {
                State = CharacterState.Moving;
                var cameraForward = _mainCamera.transform.forward;
                var cameraRight = _mainCamera.transform.right;

                cameraForward.y = cameraRight.y = 0f;
                cameraForward = cameraForward.normalized;
                cameraRight = cameraRight.normalized;

                // Вычисляем направление без нормализации
                var adjustedDirection = cameraForward * moveDirection.z + cameraRight * moveDirection.x;
                // Учитываем величину входного вектора
                var inputMagnitude = moveDirection.magnitude; // Величина входного вектора (от 0 до 1)
                Log.Warn($"Adjusted Direction: {adjustedDirection}, Input Magnitude: {inputMagnitude}");

                // Целевая скорость зависит от величины ввода
                var targetVelocity = adjustedDirection.normalized * moveSpeed * inputMagnitude;
                _currentVelocity = Vector3.Lerp(_currentVelocity, targetVelocity, acceleration);
                _rb.linearVelocity = new Vector3(_currentVelocity.x, _rb.linearVelocity.y, _currentVelocity.z);

                // Поворот в сторону движения, только если направление не нулевое
                if (adjustedDirection != Vector3.zero)
                {
                    var targetRotation = Quaternion.LookRotation(adjustedDirection.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                }
            }
            else
            {
                State = CharacterState.Idle;
                _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, acceleration);
                _rb.linearVelocity = new Vector3(_currentVelocity.x, _rb.linearVelocity.y, _currentVelocity.z);
            }
        }

        public ICharacterInteractor GetInteractor() => _interactor;
        public void SetState(CharacterState state) => State = state;
    }
}
