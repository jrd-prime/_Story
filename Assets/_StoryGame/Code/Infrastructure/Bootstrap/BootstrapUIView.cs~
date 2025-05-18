using System;
using _StoryGame.Gameplay.Extensions;
using _StoryGame.Infrastructure.Bootstrap.Interfaces;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Infrastructure.Bootstrap
{
    public sealed class BootstrapUIView : UIView<IBootstrapUIController>
    {
        private const string AppName = "App name";
        private const string BootstrapContainerId = "bootstrap-container";
        private const string LoadingLabelId = "desc-label";
        private const string AppNameLabelId = "title-label";

        private readonly CompositeDisposable _disposables = new();

        private VisualElement _container;
        private Label _appName;
        private Label _loadingLabel;

        protected override void InitElements()
        {
            _container = Root.GetVisualElement<VisualElement>("bootstrap-container", name);
            _loadingLabel = Root.GetVisualElement<Label>("desc-label", name);
            _appName = Root.GetVisualElement<Label>("title-label", name);

            _appName.text = AppName;
        }

        protected override void Subscribe()
        {
            ViewModel.LoadingText
                .Subscribe(OnSetDesc)
                .AddTo(_disposables);

            ViewModel.Opacity
                .Subscribe(OnSetOpacity)
                .AddTo(_disposables);

            ViewModel.OnClear
                .Subscribe(OnClear)
                .AddTo(_disposables);
        }

        private void OnSetDesc(string value)
        {
            if (_loadingLabel == null)
                throw new NullReferenceException("Loading label is null. " + nameof(BootstrapUIView));
            _loadingLabel.text = !string.IsNullOrEmpty(value) ? value : "Not set";
        }

        private void OnSetOpacity(float value)
        {
            if (_container == null)
                throw new NullReferenceException("Bootstrap container is null. " + nameof(BootstrapUIView));
            _container.style.opacity = value;
        }

        private void OnClear(Unit _)
        {
            if (_loadingLabel == null)
                throw new NullReferenceException("Loading label is null. " + nameof(BootstrapUIView));
            _loadingLabel.text = _appName.text = string.Empty;
        }

        private void OnDestroy() => _disposables?.Dispose();
    }
}
