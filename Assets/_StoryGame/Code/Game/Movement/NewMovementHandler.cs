using System;
using System.Runtime.CompilerServices;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Interfaces;
using _StoryGame.Game.Managers.Inerfaces;
using _StoryGame.Infrastructure.Logging;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using VContainer;

namespace _StoryGame.Game.Movement
{
    public sealed class NewMovementHandler : MonoBehaviour, IMovementHandler
    {
        public ReadOnlyReactiveProperty<Vector3> DestinationPoint => _destinationPoint;
        public ReadOnlyReactiveProperty<Vector3> MoveDirection => _moveDirection;


        private bool _isTouchActive;

        private readonly ReactiveProperty<Vector3> _destinationPoint = new(Vector3.zero);
        private readonly ReactiveProperty<Vector3> _moveDirection = new(Vector3.zero);
        private readonly ReactiveProperty<bool> _isTouchVisible = new(false);
        private readonly ReactiveProperty<Vector2> _ringPosition = new(Vector2.zero);

        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private LayerMask groundLayer;

        private IJLog _log;
        private IPlayer _player;
        private ICameraManager _cameraManager;
        private Vector2 _startTouchPosition;
        private Vector2 _moveInput;

        private EMoveState _currentMoveProcessorState;

        private readonly CompositeDisposable _disposables = new();
        private IPublisher<IMovementHandlerMsg> _selfMsgPublisher;

        [Inject]
        private void Construct(IJLog log, ICameraManager cameraManager,
            IPublisher<IMovementHandlerMsg> selfMsgPublisher,
            ISubscriber<IMovementProcessorMsg> movementProcessorMsgSubscriber)
        {
            _log = log;
            _cameraManager = cameraManager;

            _selfMsgPublisher = selfMsgPublisher;

            movementProcessorMsgSubscriber
                .Subscribe(OnMovementProcessorMessage)
                .AddTo(_disposables);
        }

        private void OnMovementProcessorMessage(IMovementProcessorMsg value)
        {
            if (value is MovementProcessorStateMsg msg)
                _currentMoveProcessorState = msg.State;
        }


        private void Start()
        {
            if (!mainCamera)
                throw new NullReferenceException($"MainCamera is null. {this}");
        }

        private void Update()
        {
            if (HasNoInput())
                return;

            if (_currentMoveProcessorState is EMoveState.MoveToInteractable)
            {
                _log.Debug("MoveState is MoveToInteractable, SKIP CLICK.");
                return;
            }

            if (TryGetInputPosition(out var inputPosition))
                HandleInput(inputPosition);
        }

        private static bool TryGetInputPosition(out Vector2 clickPosition)
        {
            clickPosition = default;

#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
            {
                clickPosition = Input.mousePosition;
                return true;
            }
#endif

#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                clickPosition = Input.GetTouch(0).position;
                return true;
            }
#endif

            return false;
        }

        private void HandleInput(Vector2 inputPosition)
        {
            if (_isTouchActive)
                return;

            if (!mainCamera)
            {
                _log.Error("Cannot process world click: Main Camera is null!");
                ResetTouch();
                return;
            }

            // _log.Debug($"Input detected at screen position: {inputPosition}");

            if (IsClickOverUI())
            {
                // _log.Debug("Click on UI, ignoring.");
                ResetTouch();
                return;
            }

            _isTouchActive = true;
            _startTouchPosition = inputPosition;

            Ray ray = mainCamera.ScreenPointToRay(inputPosition);

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, interactableLayer))
            {
                _log.Debug($"Hit Interactable object: {hit.collider.gameObject.name}");

                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    _selfMsgPublisher.Publish(new MoveToInteractableHandlerMsg(interactable));

                    _log.Debug($"Interacted with object: {interactable.Name}");
                    // interactable.Interact();
                }

                ResetTouch();
                return;
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                _log.Debug($"Hit Ground object: {hit.collider.gameObject.name}");

                if (NavMesh.SamplePosition(hit.point, out var navMeshHit, 0.5f, NavMesh.AllAreas))
                    _selfMsgPublisher.Publish(new MoveToPointHandlerMsg(navMeshHit.position));
                else _log.Debug("Clicked position is not valid on NavMesh!");
            }
            else _log.Debug("No hit on Ground layer!");

            ResetTouch();
        }

        private void ResetTouch()
        {
            _isTouchActive = false;
            _moveInput = Vector2.zero;
        }

        #region Conditions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasNoInput() =>
            !Input.GetMouseButtonDown(0) && Input.touchCount == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsClickOverUI() =>
            EventSystem.current && EventSystem.current.IsPointerOverGameObject();

        #endregion
    }

    public interface IMovementProcessorMsg : IJMessage
    {
    }

    public record MovementProcessorStateMsg(EMoveState State) : IMovementProcessorMsg
    {
        public string Name => nameof(MovementProcessorStateMsg);
        public EMoveState State { get; } = State;
    }

    public record MoveToPointHandlerMsg(Vector3 Position) : IMovementHandlerMsg
    {
        public string Name => nameof(MoveToPointHandlerMsg);
        public Vector3 Position { get; } = Position;
    }

    public interface IMovementHandlerMsg : IJMessage
    {
    }

    public record MoveToInteractableHandlerMsg(IInteractable Interactable) : IMovementHandlerMsg
    {
        public string Name => nameof(MoveToInteractableHandlerMsg);
        public IInteractable Interactable { get; } = Interactable;
    }
}
