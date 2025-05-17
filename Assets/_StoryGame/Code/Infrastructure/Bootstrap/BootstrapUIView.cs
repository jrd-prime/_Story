using System;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Infrastructure.Bootstrap
{
    public sealed class BootstrapUIView : MonoBehaviour
    {
        private const string AppName = "App name";
        private const string BootstrapContainerId = "bootstrap-container";
        private const string LoadingLabelId = "desc-label";
        private const string AppNameLabelId = "title-label";

        [Inject] private IBootstrapUIController _controller;
        private readonly CompositeDisposable _disposables = new();

        private VisualElement _container;
        private Label _appName;
        private Label _loadingLabel;

        private async void Start()
        {
            var uiDoc = GetComponent<UIDocument>();
            await UIToolkitReadyAwaiter.WaitForReadyAsync(uiDoc);

            var root = uiDoc.rootVisualElement;

            if (root == null)
                throw new NullReferenceException("RootVisualElement is null on start in " + name);

            _container = root.Q<VisualElement>("bootstrap-container") ??
                         throw new NullReferenceException("Bootstrap container is null. " + nameof(BootstrapUIView));
            _loadingLabel = root.Q<Label>("desc-label") ??
                            throw new NullReferenceException("Loading label is null. " + nameof(BootstrapUIView));
            _appName = root.Q<Label>("title-label") ??
                       throw new NullReferenceException("App name label is null. " + nameof(BootstrapUIView));

            _appName.text = AppName;

            Subscribe();
        }

        private void Subscribe()
        {
            if (_controller == null)
                throw new NullReferenceException("BootstrapUIController is null. " + nameof(BootstrapUIView));

            _controller.LoadingText
                .Subscribe(OnSetDesc)
                .AddTo(_disposables);

            _controller.Opacity
                .Subscribe(OnSetOpacity)
                .AddTo(_disposables);

            _controller.OnClear
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
