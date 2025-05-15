using System;
using _StoryGame.Gameplay.Extensions;
using ModestTree;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace _StoryGame
{
    public class GameUIToolkitView : MonoBehaviour
    {
        [Inject] private FullScreenMovementViewModel ViewModel;

        // Movement
        private VisualElement _movementRoot;
        private VisualElement _ring;

        // Buttons
        private Button _menuButton;

        private VisualElement RootVisualElement;
        private CompositeDisposable Disposables = new();

        private void Start()
        {
            RootVisualElement = GetComponent<UIDocument>().rootVisualElement;
            if (RootVisualElement == null) throw new NullReferenceException("RootVisualElement is null");
            if (ViewModel == null) throw new NullReferenceException("ViewModel is null");
            InitElements();
            Init();
        }

        protected void InitElements()
        {
            // var safeZoneOffset = ScreenHelper.GetSafeZoneOffset(800f, 360f);
            // RootVisualElement.style.marginLeft = safeZoneOffset.x >= 16 ? safeZoneOffset.x : 16;
            // RootVisualElement.style.marginTop = safeZoneOffset.y;

            _movementRoot = RootVisualElement.GetVisualElement<VisualElement>(UIConst.MovementRootIDName, name);
            _ring = RootVisualElement.GetVisualElement<VisualElement>(UIConst.FullScreenRingIDName, name);
        }

        protected void Init()
        {
            _movementRoot.RegisterCallback<PointerDownEvent>(OnPointerDown);
            _movementRoot.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            _movementRoot.RegisterCallback<PointerUpEvent>(OnPointerUp);
            _movementRoot.RegisterCallback<PointerOutEvent>(OnPointerCancel);

            // Movement
            ViewModel.IsTouchPositionVisible.Subscribe(IsTouchPositionVisible).AddTo(Disposables);
            ViewModel.RingPosition.Subscribe(SetRingPosition).AddTo(Disposables);
        }

        private void SetRingPosition(Vector2 position)
        {
            _ring.style.left = position.x;
            _ring.style.top = position.y;
        }

        private void IsTouchPositionVisible(bool value) =>
            _ring.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;

        private void OnPointerCancel(PointerOutEvent evt) => ViewModel.OnOutEvent(evt);

        private void OnPointerDown(PointerDownEvent evt)
        {
            ViewModel.OnDownEvent(evt);
        }

        private void OnPointerMove(PointerMoveEvent evt) => ViewModel.OnMoveEvent(evt);
        private void OnPointerUp(PointerUpEvent evt) => ViewModel.OnUpEvent(evt);
    }
}
