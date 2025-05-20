using System;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Game.Managers.Inerfaces;
using R3;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Game.Movement
{
    public sealed class NewMovementHandler : MonoBehaviour, IMovementHandler, IInitializable
    {
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

        // Поля для настройки в инспекторе
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private LayerMask _interactableLayer;
        [SerializeField] private LayerMask _groundLayer;
        private IPlayer _player;
        private ICameraManager _cameraManager;
        private NavMeshAgent _navMeshAgent;

        [Inject]
        private void Construct(IPlayer player, ICameraManager cameraManager)
        {
            _player = player;
            _cameraManager = cameraManager;
        }

        public void Initialize()
        {
        }

        private void Start()
        {
            Debug.Log("<color=green>NewMovementHandler started</color>");
            // Инициализация камеры
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null)
                {
                    Debug.LogError("Main Camera is null! Please assign in Inspector.");
                }
            }

            _navMeshAgent = _player.NavMeshAgent;
            // Проверяем NavMeshAgent
            if (_navMeshAgent == null)
            {
                Debug.LogError("NavMeshAgent is null! Please assign in Inspector.");
            }
            _navMeshAgent.enabled = true;
        }

        private void Update()
        {
            // Проверяем клик мыши или касание
            Vector2 inputPosition = Vector2.zero;
            bool inputDetected = false;

            // Для мыши
            if (Input.GetMouseButtonDown(0))
            {
                inputPosition = Input.mousePosition;
                inputDetected = true;
            }
            // Для касаний (мобильные устройства)
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                inputPosition = Input.GetTouch(0).position;
                inputDetected = true;
            }

            if (inputDetected)
            {
                HandleInput(inputPosition);
            }
        }

        private void HandleInput(Vector2 inputPosition)
        {
            if (_isTouchActive) return;

            _isTouchActive = true;
            _startTouchPosition = inputPosition;
            Debug.Log($"Input detected at screen position: {inputPosition}");

            // Пропускаем, если клик на UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Click on UI, ignoring.");
                ResetTouch();
                return;
            }

            // Проверяем клик по миру
            if (_mainCamera == null)
            {
                Debug.LogError("Cannot process world click: Main Camera is null!");
                ResetTouch();
                return;
            }

            Ray ray = _mainCamera.ScreenPointToRay(inputPosition);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

            // Проверяем Interactable слой
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _interactableLayer))
            {
                Debug.Log(
                    $"Hit Interactable object: {hit.collider.gameObject.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                    ResetTouch();
                    return;
                }
            }

            // Проверяем Ground слой
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundLayer))
            {
                Debug.Log(
                    $"Hit Ground object: {hit.collider.gameObject.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
                NavMeshHit navMeshHit;
                if (NavMesh.SamplePosition(hit.point, out navMeshHit, 1.0f, NavMesh.AllAreas))
                {
                    Debug.Log($"Valid NavMesh position: {navMeshHit.position}");
                    MoveToPosition(navMeshHit.position);
                }
                else
                {
                    Debug.Log("Clicked position is not valid on NavMesh!");
                }
            }
            else
            {
                Debug.Log("No hit on Ground layer!");
            }

            ResetTouch();
        }

        private void SetMoveDirection(Vector3 value) => _moveDirection.Value = value;

        public void OnPointerDown(PointerDownEvent evt)
        {
            // Оставляем пустым, так как теперь используем Update
        }

        public void OnPointerMove(PointerMoveEvent evt)
        {
            // Оставляем пустым, так как движение обрабатывается в Update
        }

        public void OnPointerUp(PointerUpEvent evt)
        {
            // Оставляем пустым, так как сброс обрабатывается в Update
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

        private void MoveToPosition(Vector3 targetPosition)
        {
            if (_navMeshAgent != null)
            {
                Debug.Log($"Moving to position: {targetPosition}");
                _navMeshAgent.SetDestination(targetPosition);
            }
            else
            {
                Debug.LogError("NavMeshAgent is null, cannot move!");
            }
        }
    }

    public interface IInteractable
    {
        void Interact();
    }
}
