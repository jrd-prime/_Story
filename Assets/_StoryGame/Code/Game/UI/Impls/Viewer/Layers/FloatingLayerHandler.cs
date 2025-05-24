using System.Collections.Generic;
using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Data;
using _StoryGame.Game.Extensions;
using _StoryGame.Game.Interactables.Inspect;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.UI.Impls.Viewer.Layers
{
    // TODO переделать
    public sealed class FloatingLayerHandler : UIViewerHandlerBase, IUIViewerLayerHandler
    {
        private const string LeftId = "left";
        private const string CenterId = "center";
        private const string RightId = "right";

        private readonly Dictionary<FloatingWindowType, VisualTreeAsset> _floatingWindowsAssets = new();
        private readonly Dictionary<FloatingWindowType, VisualElement> _windows = new();
        private VisualElement _left;
        private VisualElement _center;
        private VisualElement _right;

        public FloatingLayerHandler(IObjectResolver resolver, VisualElement layerBack) : base(resolver, layerBack)
        {
        }

        protected override void PreInitialize()
        {
            var uiSettings = SettingsProvider.GetSettings<UISettings>();
            var floatingWindowsData = uiSettings.FloatingWindowDataVo;

            foreach (var windowData in floatingWindowsData.FloatingWindowDataVo)
                _floatingWindowsAssets.Add(windowData.floatingWindowType, windowData.visualTreeAsset);

            Log.Debug("FloatingLayerHandler initialized with " + _floatingWindowsAssets.Count + " windows.");
        }

        protected override void InitElements()
        {
            _left = MainContainer.GetVisualElement<VisualElement>(LeftId, nameof(FloatingLayerHandler));
            _center = MainContainer.GetVisualElement<VisualElement>(CenterId, nameof(FloatingLayerHandler));
            _right = MainContainer.GetVisualElement<VisualElement>(RightId, nameof(FloatingLayerHandler));

            InitializeWindows();
        }

        private void InitializeWindows()
        {
            foreach (var window in _floatingWindowsAssets)
            {
                var instance = window.Value.Instantiate();
                _windows.Add(window.Key, instance);
            }
        }

        protected override void Subscribe()
        {
        }

        protected override void Unsubscribe()
        {
        }


        public void ShowHasLootWindow(ShowHasLootWindowMsg msg)
        {
            if (!HasWindow(msg.WindowType))
                throw new KeyNotFoundException($"No window for type: {msg.WindowType}");

            _center.Clear();

            var window = _windows[msg.WindowType];

            var label = window.GetVisualElement<Label>("label", window.name);
            var close = window.GetVisualElement<Button>("close", window.name);
            var search = window.GetVisualElement<Button>("search", window.name);

            label.text = "Loot";


            var closeHandler =
                new ClickCompletionHandler<DialogResult>(close, DialogResult.Close, msg.CompletionSource, _center);
            var searchHandler =
                new ClickCompletionHandler<DialogResult>(search, DialogResult.Search, msg.CompletionSource, _center);

            closeHandler.Register();
            searchHandler.Register();

            _center.Add(window);
        }

        public void ShowNoLootWindow(ShowNoLootWindowMsg msg)
        {
            if (!HasWindow(msg.WindowType))
                throw new KeyNotFoundException($"No window for type: {msg.WindowType}");

            _center.Clear();

            var window = _windows[msg.WindowType];

            var label = window.GetVisualElement<Label>("label", window.name);
            var close = window.GetVisualElement<Button>("close", window.name);

            label.text = "No Loot";

            var closeHandler =
                new ClickCompletionHandler<DialogResult>(close, DialogResult.Close, msg.CompletionSource, _center);

            closeHandler.Register();

            _center.Add(window);
        }

        private bool HasWindow(FloatingWindowType windowType) => _windows.ContainsKey(windowType);
    }

    public enum PositionType
    {
        Left,
        Center,
        Right
    }

    public enum FloatingWindowType
    {
        HasLoot,
        NoLoot
    }
}
