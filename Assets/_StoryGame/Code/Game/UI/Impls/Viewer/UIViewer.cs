using System;
using System.Collections.Generic;
using _StoryGame.Core.HSM.Impls.States;
using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Data.Const;
using _StoryGame.Game.Extensions;
using _StoryGame.Game.UI.Impls.Viewer.Layers;
using _StoryGame.Game.UI.Messages;
using _StoryGame.Infrastructure.Logging;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.UI.Impls.Viewer
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class UIViewer : MonoBehaviour, IUIViewer
    {
        private const string LayerBackId = "layer-back";
        private const string LayerMovementId = "layer-movement";
        private const string LayerHudId = "layer-hud";
        private const string LayerFloatingId = "layer-floating";

        private IObjectResolver _resolver;
        private IJLog _log;

        private BackLayerHandler _backLayerHandler;
        private MovementLayerHandler _movementLayerHandler;
        private HUDLayerHandler _hudLayerHandler;
        private FloatingLayerHandler _floatingLayerHandler;

        private bool _isInitialized;
        private VisualElement _mainContainer;

        private readonly Dictionary<GameStateType, TemplateContainer> _viewsCache = new();
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(IObjectResolver resolver, ISubscriber<IUIViewerMessage> subscriber)
        {
            _resolver = resolver;
            _log = resolver.Resolve<IJLog>();

            subscriber
                .Subscribe(OnMessage)
                .AddTo(_disposables);
        }

        private void OnMessage(IUIViewerMessage message)
        {
            switch (message)
            {
                case InitializeViewerMessage msg:
                    Initialize(msg.Views);
                    break;
                case SwitchBaseViewMessage msg:
                    SwitchBaseViewTo(msg.StateType);
                    break;
                case ShowHasLootWindowMsg msg:
                    _floatingLayerHandler.ShowHasLootWindow(msg);
                    break;
                case ShowNoLootWindowMsg msg:
                    _floatingLayerHandler.ShowNoLootWindow(msg);
                    break;
                case ShowLootWindowMsg msg:
                    _floatingLayerHandler.ShowLootWindow(msg);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(message), message, null);
            }
        }

        private async void Awake()
        {
            var document = GetComponent<UIDocument>();
            await document.WaitForReadyAsync();

            var root = document.rootVisualElement;
            root.SetFullScreen();

            var safeZoneOffset = ScreenHelper.GetSafeZoneOffset(1600f, 720f);
            root.style.marginLeft = safeZoneOffset.x >= 16 ? safeZoneOffset.x : 16;
            root.style.marginTop = safeZoneOffset.y;

            _mainContainer = root.GetVisualElement<VisualElement>(UIConst.MainContainer, name);

            if (_mainContainer == null)
                throw new NullReferenceException($"VisualElement with ID '{UIConst.MainContainer}' not found. " +
                                                 nameof(UIViewer));

            InitializeLayers();
        }

        private void InitializeLayers()
        {
            // Back Layer
            var backLayerRoot = _mainContainer.GetVisualElement<VisualElement>(LayerBackId, name);
            _backLayerHandler = new BackLayerHandler(_resolver, backLayerRoot);

            // Movement Layer
            var movementLayerRoot = _mainContainer.GetVisualElement<VisualElement>(LayerMovementId, name);
            _movementLayerHandler = new MovementLayerHandler(_resolver, movementLayerRoot);

            // HUD Layer
            var hudLayerRoot = _mainContainer.GetVisualElement<VisualElement>(LayerHudId, name);
            _hudLayerHandler = new HUDLayerHandler(_resolver, hudLayerRoot);

            // Floating Layer
            var floatingLayerRoot = _mainContainer.GetVisualElement<VisualElement>(LayerFloatingId, name);
            _floatingLayerHandler = new FloatingLayerHandler(_resolver, floatingLayerRoot);
        }

        private void Initialize(IDictionary<GameStateType, UIViewBase> viewsCache)
        {
            foreach (var view in viewsCache)
                _viewsCache.TryAdd(view.Key, view.Value.Template);

            _isInitialized = true;
        }

        private void SwitchBaseViewTo(GameStateType state)
        {
            if (!_isInitialized)
                throw new NullReferenceException("UIViewer is not initialized. " + nameof(SwitchBaseViewTo));

            if (!_viewsCache.TryGetValue(state, out var view))
                throw new NullReferenceException($"No view for state: {state}. " + nameof(SwitchBaseViewTo));

            _log.Info($"SHOW UI FOR: {state}");

            _hudLayerHandler.SwitchViewTo(view);
        }

        private void OnDestroy()
        {
            _isInitialized = false;
            _viewsCache.Clear();
        }
    }

    public interface IUIViewer
    {
    }
}
