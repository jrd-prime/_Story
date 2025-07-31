using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Game.Character.Player.Messages;
using _StoryGame.Game.Movement.Messages;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace _StoryGame.Game.Movement
{
    public sealed class NewMovementHandler : MonoBehaviour, IMovementHandler
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private LayerMask groundLayer;

        public ReadOnlyReactiveProperty<Vector3> DestinationPoint => _destinationPoint;
        public ReadOnlyReactiveProperty<Vector3> MoveDirection => _moveDirection;


        private IJLog _log;
        private IPlayer _player;
        private IPublisher<IMovementHandlerMsg> _selfMsgPub;

        private bool _isTouchActive;
        private ECharacterState _currentPlayerState;

        private readonly ReactiveProperty<Vector3> _destinationPoint = new(Vector3.zero);
        private readonly ReactiveProperty<Vector3> _moveDirection = new(Vector3.zero);
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(
            IJLog log,
            IPublisher<IMovementHandlerMsg> selfMsgPub,
            ISubscriber<IPlayerMsg> playerMsgSub,
            ISubscriber<IInteractProcessorMsg> movementProcessorMsgSub)
        {
            _log = log;

            _selfMsgPub = selfMsgPub;

            playerMsgSub.Subscribe(
                    msg => { _currentPlayerState = ((PlayerStateMsg)msg).State; },
                    msg => msg is PlayerStateMsg)
                .AddTo(_disposables);
        }

        private void Start()
        {
            if (!mainCamera)
                throw new NullReferenceException($"MainCamera is null. {this}");
        }

        private void Update()
        {
            if (_currentPlayerState is ECharacterState.Interacting or ECharacterState.MovingToInteractable
                or ECharacterState.PopUp)
                return;

            if (HasNoInput())
                return;

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
                _log.Debug($"Hit layer Interact object: {hit.collider.gameObject.name}");

                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    _selfMsgPub.Publish(new MoveToInteractableHandlerMsg(interactable));
                    _log.Debug($"Interacted with object: {interactable.LocalizationKey}");
                }

                ResetTouch();
                return;
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                // _log.Debug($"Hit Ground object: {hit.collider.gameObject.name}");

                if (NavMesh.SamplePosition(hit.point, out var navMeshHit, 0.5f, NavMesh.AllAreas))
                {
                    _selfMsgPub.Publish(new MoveToPointHandlerMsg(navMeshHit.position));
                    // _log.Debug($"Clicked position is valid on NavMesh: {navMeshHit.position}");
                }
            }

            ResetTouch();
        }

        private void ResetTouch()
        {
            // _log.Debug("ResetTouch");
            _isTouchActive = false;
        }

        #region Conditions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HasNoInput() =>
            !Input.GetMouseButtonDown(0) && Input.touchCount == 0;

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsClickOverUI()
        {
            if (!EventSystem.current || !EventSystem.current.IsPointerOverGameObject())
                return false;

            // Debug.LogError(EventSystem.current);
            // Отладочная информация
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count <= 0)
                return true;

            StringBuilder debugInfo = new StringBuilder("Клик по UI: ");
            foreach (var result in results)
            {
                debugInfo.AppendLine();
                debugInfo.Append(
                    $"- {result.gameObject.name} (слой: {LayerMask.LayerToName(result.gameObject.layer)})");

                if (result.gameObject.transform.parent != null)
                    debugInfo.Append($", родитель: {result.gameObject.transform.parent.name}");

                var selectable = result.gameObject.GetComponent<Selectable>();
                if (selectable != null)
                    debugInfo.Append($", компонент: {selectable.GetType().Name}");
            }

            Debug.Log(debugInfo.ToString());

            return true;
        }

        #endregion
    }

    public interface IMovementHandler
    {
        ReadOnlyReactiveProperty<Vector3> DestinationPoint { get; }
        ReadOnlyReactiveProperty<Vector3> MoveDirection { get; }
    }
}
