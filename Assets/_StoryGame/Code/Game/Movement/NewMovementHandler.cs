using System;
using System.Runtime.CompilerServices;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Interactables.Interfaces;
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

        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private LayerMask groundLayer;

        private IJLog _log;
        private IPlayer _player;

        private EMoveState _currentMoveProcessorState;

        private readonly CompositeDisposable _disposables = new();
        private IPublisher<IMovementHandlerMsg> _selfMsgPub;

        [Inject]
        private void Construct(
            IJLog log,
            ICameraManager cameraManager,
            IPublisher<IMovementHandlerMsg> selfMsgPub,
            ISubscriber<IMovementProcessorMsg> movementProcessorMsgSub)
        {
            _log = log;

            _selfMsgPub = selfMsgPub;

            movementProcessorMsgSub
                .Subscribe(msg =>
                {
                    _currentMoveProcessorState = msg is MovementProcessorStateMsg stateMsg
                        ? stateMsg.State
                        : _currentMoveProcessorState;
                })
                .AddTo(_disposables);
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

            if (_currentMoveProcessorState is EMoveState.ToInteract)
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

            if (IsClickOverUI())
            {
                _log.Debug("Click on UI, ignoring.");
                ResetTouch();
                return;
            }

            _isTouchActive = true;

            Ray ray = mainCamera.ScreenPointToRay(inputPosition);

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, interactableLayer))
            {
                // _log.Debug($"Hit layer Interactable object: {hit.collider.gameObject.name}");

                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    _selfMsgPub.Publish(new MoveToInteractableHandlerMsg(interactable));
                    // _log.Debug($"Interacted with object: {interactable.Name}");
                }

                ResetTouch();
                return;
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                // _log.Debug($"Hit Ground object: {hit.collider.gameObject.name}");

                if (NavMesh.SamplePosition(hit.point, out var navMeshHit, 0.5f, NavMesh.AllAreas))
                {
                    // _log.Debug($"Clicked position is valid on NavMesh: {navMeshHit.position}");
                    _selfMsgPub.Publish(new MoveToPointHandlerMsg(navMeshHit.position));
                }
            }

            ResetTouch();
        }

        private void ResetTouch() => _isTouchActive = false;

        #region Conditions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasNoInput() =>
            !Input.GetMouseButtonDown(0) && Input.touchCount == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsClickOverUI() =>
            EventSystem.current && EventSystem.current.IsPointerOverGameObject();

        #endregion
    }
}
