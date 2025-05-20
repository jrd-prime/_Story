using System;
using System.Collections.Generic;
using _StoryGame.Core.Managers.HSM.Impls.States;
using _StoryGame.Data.UI;
using _StoryGame.Gameplay.Extensions;
using _StoryGame.Gameplay.UI.Impls.Viewer.Layers;
using _StoryGame.Gameplay.UI.Interfaces;
using _StoryGame.Gameplay.UI.Messages;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Gameplay.UI.Impls.Viewer
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class UIViewer : MonoBehaviour, IUIViewer
    {
        private bool _isInitialized;
        private readonly Dictionary<GameStateType, TemplateContainer> _viewsCache = new();
        private VisualElement _viewerRoot;
        private VisualElement _viewerMainContainer;
        private VisualElement _movementArea;
        private VisualElement _ring;

        private readonly CompositeDisposable _disposables = new();

        private BackLayerHandler _backLayerHandler;
        private MovementLayerHandler _movementLayerHandler;
        private HUDLayerHandler _hudLayerHandler;
        private FloatingLayerHandler _floatingLayerHandler;
        private IObjectResolver _resolver;

        [Inject]
        private void Construct(IObjectResolver resolver, ISubscriber<IUIViewerMessage> subscriber)
        {
            _resolver = resolver;
            subscriber.Subscribe(OnMessage).AddTo(_disposables);
        }

        private void OnMessage(IUIViewerMessage message)
        {
            switch (message)
            {
                case ShowPopUpMessage msg:
                    _floatingLayerHandler.ShowFloatingWindow(msg.Id, msg.Command, positionType: msg.PositionType);
                    break;
                case ResetPopUpMessage msg:
                    _floatingLayerHandler.ResetPopUp(msg.Id);
                    break;
            }
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

            _viewerMainContainer = _viewerRoot.GetVisualElement<VisualElement>(UIConst.MainContainer, name);

            InitializeLayers();
        }

        private void InitializeLayers()
        {
            // Back Layer
            var backLayerRoot = _viewerMainContainer.GetVisualElement<VisualElement>("layer-back", name);
            _backLayerHandler = new BackLayerHandler(_resolver, backLayerRoot);

            // Movement Layer
            var movementLayerRoot = _viewerMainContainer.GetVisualElement<VisualElement>("layer-movement", name);
            _movementLayerHandler = new MovementLayerHandler(_resolver, movementLayerRoot);

            // HUD Layer
            var _hudLayerRoot = _viewerMainContainer.GetVisualElement<VisualElement>("layer-hud", name);
            _hudLayerHandler = new HUDLayerHandler(_resolver, _hudLayerRoot);

            // Floating Layer
            var _floatingLayerRoot = _viewerMainContainer.GetVisualElement<VisualElement>("layer-floating", name);
            _floatingLayerHandler = new FloatingLayerHandler(_resolver, _floatingLayerRoot);
        }

        public void Initialize(Dictionary<GameStateType, UIViewBase> viewsCache)
        {
            foreach (var view in viewsCache)
                _viewsCache.TryAdd(view.Key, view.Value.Template);

            _isInitialized = true;
        }

        public void SwitchTo(GameStateType state)
        {
            if (!_isInitialized)
                throw new NullReferenceException("UIViewer is not initialized. " + nameof(SwitchTo));

            if (!_viewsCache.TryGetValue(state, out var view))
                throw new NullReferenceException($"No view for state: {state}. " + nameof(SwitchTo));

            _hudLayerHandler.SwitchViewTo(view);
        }

        private void OnDestroy()
        {
            _isInitialized = false;
            _viewsCache.Clear();
        }
    }
}
