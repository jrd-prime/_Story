using System;
using System.Collections.Generic;
using _StoryGame.Data;
using _StoryGame.Gameplay.Extensions;
using _StoryGame.Gameplay.UI.Interfaces;
using R3;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Gameplay.UI.Impls.Viewer.Layers
{
    public sealed class FloatingLayerHandler : UIViewerHandlerBase, IUIViewerLayerHandler
    {
        private readonly Dictionary<FloatingWindowType, VisualTreeAsset> _floatingWindowsAssets = new();
        private readonly Dictionary<FloatingWindowType, VisualElement> _windows = new();
        private VisualElement _left;
        private VisualElement _center;
        private VisualElement _right;

        // TODO сделать хранение элементов для типов окон
        private ReactiveCommand _msgCommand;
        private Button _btn;
        private Label _label;

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
            _left = MainContainer.GetVisualElement<VisualElement>("left", nameof(FloatingLayerHandler));
            _center = MainContainer.GetVisualElement<VisualElement>("center", nameof(FloatingLayerHandler));
            _right = MainContainer.GetVisualElement<VisualElement>("right", nameof(FloatingLayerHandler));

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

        public void ShowFloatingWindow(
            string text,
            ReactiveCommand callback,
            FloatingWindowType windowType = FloatingWindowType.PreInteract,
            PositionType positionType = PositionType.Center
        )
        {
            var window = windowType switch
            {
                FloatingWindowType.PreInteract => _windows.GetValueOrDefault(windowType),
                _ => throw new ArgumentOutOfRangeException(nameof(windowType), windowType, null)
            };

            if (window == null)
                throw new NullReferenceException("Floating window is null. " + nameof(ShowFloatingWindow));

            switch (positionType)
            {
                case PositionType.Left:
                    break;
                case PositionType.Center:
                    _center.Clear();
                    PrepareWindow(window, text, callback);
                    _center.Add(window);
                    break;
                case PositionType.Right:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(positionType), positionType, null);
            }
        }

        // TODO переделать под разные окна и тд. сделать фабрику
        private void PrepareWindow(VisualElement window, string text, ReactiveCommand callback)
        {
            _msgCommand = callback;
            _label = window.GetVisualElement<Label>("label", window.name);
            _label.text = text;
            _btn = window.GetVisualElement<Button>("btn", window.name);

            _btn.RegisterCallback<ClickEvent>(OnPopUpBtnClick);
        }

        public void ResetPopUp(string msgId)
        {
            _btn.UnregisterCallback<ClickEvent>(OnPopUpBtnClick);
            _center.Clear();
        }

        private void OnPopUpBtnClick(ClickEvent _)
        {
            _msgCommand.Execute(Unit.Default);
            _btn.UnregisterCallback<ClickEvent>(OnPopUpBtnClick);
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
        PreInteract
    }
}
