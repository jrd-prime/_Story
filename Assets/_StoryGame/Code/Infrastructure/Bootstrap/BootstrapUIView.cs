using _StoryGame.Gameplay.Extensions;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

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

        private void Awake()
        {
            var root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            _container = root.GetVisualElement<VisualElement>(BootstrapContainerId, name);
            _loadingLabel = root.GetVisualElement<Label>(LoadingLabelId, name);
            _appName = root.GetVisualElement<Label>(AppNameLabelId, name);

            _appName.text = AppName;

            Subscribe();
        }

        private void Subscribe()
        {
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

        private void OnSetDesc(string value) => _loadingLabel.text = !string.IsNullOrEmpty(value) ? value : "Not set";
        private void OnSetOpacity(float value) => _container.style.opacity = value;
        private void OnClear(Unit _) => _loadingLabel.text = _appName.text = string.Empty;
        private void OnDestroy() => _disposables?.Dispose();
    }
}
