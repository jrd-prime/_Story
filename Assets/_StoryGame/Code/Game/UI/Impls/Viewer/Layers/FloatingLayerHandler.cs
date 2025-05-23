using System;
using System.Collections.Generic;
using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Data;
using _StoryGame.Game.Extensions;
using _StoryGame.Game.Interactables.Inspect;
using _StoryGame.Game.UI.Messages;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.UI.Impls.Viewer.Layers
{
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

        // TODO сделать хранение элементов для типов окон
        private ReactiveCommand _msgCommand;
        private Button close;
        private Label _label;
        private Button search;
        private FloatingWindowFactory _floatingWindowFactory;

        public FloatingLayerHandler(IObjectResolver resolver, VisualElement layerBack) : base(resolver, layerBack)
        {
        }

        protected override void PreInitialize()
        {
            var uiSettings = SettingsProvider.GetSettings<UISettings>();
            var floatingWindowsData = uiSettings.FloatingWindowDataVo;

            foreach (var windowData in floatingWindowsData.FloatingWindowDataVo)
                _floatingWindowsAssets.Add(windowData.floatingWindowType, windowData.visualTreeAsset);

            _floatingWindowFactory = new FloatingWindowFactory(_windows);
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

        public void ShowFloatingWindow(ShowFloatingWindowMsg<DialogResult> msg)
        {
            var window = _floatingWindowFactory.CreateAndFill(msg);


            _center.Clear();


            _label = window.GetVisualElement<Label>("label", window.name);
            _label.text = msg.Text;

            close = window.GetVisualElement<Button>("close", window.name);
            search = window.GetVisualElement<Button>("search", window.name);

            close.RegisterCallback<ClickEvent>(_ => OnClo(msg.CompletionSource));
            search.RegisterCallback<ClickEvent>(_ => OnSer(msg.CompletionSource));


            _center.Add(window);
        }

        private void OnClo(UniTaskCompletionSource<DialogResult> source)
        {
            Debug.Log("OnClo");
            source.TrySetResult(DialogResult.Close);
            close.UnregisterCallback<ClickEvent>(_ => OnClo(source));
            _center.Clear();
        }

        private void OnSer(UniTaskCompletionSource<DialogResult> source)
        {
            Debug.Log("OnSer");
            source.TrySetResult(DialogResult.Search);
            search.UnregisterCallback<ClickEvent>(_ => OnSer(source));
            _center.Clear();
        }
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
