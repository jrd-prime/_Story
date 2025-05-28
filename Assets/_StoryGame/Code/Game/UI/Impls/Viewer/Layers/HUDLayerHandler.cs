using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Data.Const;
using _StoryGame.Data.UI;
using _StoryGame.Game.Extensions;
using _StoryGame.Infrastructure.Tools;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Game.UI.Impls.Viewer.Layers
{
    public sealed class HUDLayerHandler : UIViewerHandlerBase, IUIViewerLayerHandler
    {
        private const string FpsLabelId = "fps";

        private FPSCounter _fpsCounter;
        private Label _fpsLabel;
        private VisualElement _currentViewMainContainer = null;

        public HUDLayerHandler(IObjectResolver resolver, VisualElement layerBack) : base(resolver, layerBack)
        {
        }

        protected override void ResolveDependencies(IObjectResolver resolver)
        {
            _fpsCounter = resolver.Resolve<FPSCounter>();
        }

        protected override void InitElements()
        {
            _fpsLabel = GetElement<Label>(FpsLabelId);
        }

        protected override void Subscribe()
        {
            _fpsCounter.Fps
                .Subscribe(ShowFps)
                .AddTo(Disposables);
        }

        protected override void Unsubscribe()
        {
        }

        private void ShowFps(float value)
        {
            UniTask.Post(
                () => _fpsLabel.text = value.ToString("F1")
            );
        }

        // TODO интересное переключение с анимациями
        public void SwitchViewTo(TemplateContainer value)
        {
            if (_currentViewMainContainer != null)
                _currentViewMainContainer.style.display = DisplayStyle.None;

            MainContainer.Clear();
            MainContainer.Add(value);
            var newView = value.GetVisualElement<VisualElement>(UIConst.MainContainer, nameof(HUDLayerHandler));
            newView.style.display = DisplayStyle.Flex;
            _currentViewMainContainer = newView;
        }
    }
}
