using System;
using System.Collections.Generic;
using _StoryGame.Core.Managers.HSM.Impls.States;
using _StoryGame.Data.UI;
using _StoryGame.Gameplay.Extensions;
using _StoryGame.Gameplay.Movement;
using _StoryGame.Gameplay.UI.Interfaces;
using _StoryGame.Infrastructure.Tools;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Gameplay.UI.Impls
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class UIViewer : MonoBehaviour, IUIViewer
    {
        private bool _isInitialized;
        private readonly Dictionary<GameStateType, TemplateContainer> _viewsCache = new();
        private VisualElement _viewerRoot;
        private VisualElement _layer0;
        private VisualElement _layer1;
        private VisualElement _layer2;
        private VisualElement _mainContainer;
        private VisualElement _movementArea;
        private VisualElement _ring;
        private IMovementHandler _movementHandler;

        private readonly CompositeDisposable _disposables = new();
        private FPSCounter _fpsCounter;
        private Label _fpsLabel;
        private VisualElement _2main;

        [Inject]
        private void Construct(IMovementHandler movementHandler, FPSCounter fpsCounter)
        {
            _movementHandler = movementHandler;
            _fpsCounter = fpsCounter;
        }

        private async void Awake()
        {
            var document = GetComponent<UIDocument>();
            await document.WaitForReadyAsync();

            _viewerRoot = document.rootVisualElement;
            _viewerRoot.SetFullScreen();

            var safeZoneOffset = ScreenHelper.GetSafeZoneOffset(1600f, 720f);
            _viewerRoot.style.marginLeft = safeZoneOffset.x >= 16 ? safeZoneOffset.x : 16;
            _viewerRoot.style.marginTop = safeZoneOffset.y;

            _mainContainer = _viewerRoot.GetVisualElement<VisualElement>(UIConst.MainContainer, name);

            _layer0 = _mainContainer.GetVisualElement<VisualElement>("layer-0", name);
            _layer1 = _mainContainer.GetVisualElement<VisualElement>("layer-1", name);
            _layer2 = _mainContainer.GetVisualElement<VisualElement>("layer-2", name);

            _2main = _layer2.GetVisualElement<VisualElement>(UIConst.MainContainer, name);

            _layer0.SetFullScreen();
            _layer1.SetFullScreen();
            _layer2.SetFullScreen();

            _fpsLabel = _layer2.GetVisualElement<Label>("fps", name);
            _fpsCounter.Fps.Subscribe(ShowFps).AddTo(_disposables);

            //TODO movement вынести
            
            _movementArea = _layer1.GetVisualElement<VisualElement>("movement-area", name);
            _ring = _layer1.GetVisualElement<VisualElement>("ring", name);

            if (_movementHandler == null)
                throw new NullReferenceException(nameof(_movementHandler));

            _movementHandler.IsTouchVisible.Subscribe(IsTouchPositionVisible).AddTo(_disposables);
            _movementHandler.RingPosition.Subscribe(SetRingPosition).AddTo(_disposables);

            _movementArea.RegisterCallback<PointerDownEvent>(_movementHandler.OnPointerDown);
            _movementArea.RegisterCallback<PointerMoveEvent>(_movementHandler.OnPointerMove);
            _movementArea.RegisterCallback<PointerUpEvent>(_movementHandler.OnPointerUp);
            _movementArea.RegisterCallback<PointerOutEvent>(_movementHandler.OnPointerCancel);
        }

        public void Initialize(Dictionary<GameStateType, UIViewBase> viewsCache)
        {
            foreach (var view in viewsCache)
                _viewsCache.TryAdd(view.Key, view.Value.Template);

            _isInitialized = true;
        }

        private void ShowFps(float value) => _fpsLabel.text = value.ToString("F1");

        public void SwitchTo(GameStateType state)
        {
            if (!_isInitialized)
                throw new NullReferenceException("UIViewer is not initialized. " + nameof(SwitchTo));

            if (_viewsCache.TryGetValue(state, out var view))
            {
                _2main.Clear();
                _2main.Add(view);
                var viewMain = view.GetVisualElement<VisualElement>(UIConst.MainContainer, name);
                viewMain.style.display = DisplayStyle.Flex;
            }
        }

        private void SetRingPosition(Vector2 position)
        {
            _ring.style.left = position.x;
            _ring.style.top = position.y;
        }

        private void IsTouchPositionVisible(bool value) =>
            _ring.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;

        private void OnDestroy()
        {
            _isInitialized = false;
            _viewsCache.Clear();
        }
    }
}
