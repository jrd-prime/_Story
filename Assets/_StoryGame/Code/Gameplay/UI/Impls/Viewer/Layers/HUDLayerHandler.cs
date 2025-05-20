using _StoryGame.Data.UI;
using _StoryGame.Gameplay.Extensions;
using _StoryGame.Gameplay.UI.Interfaces;
using _StoryGame.Infrastructure.Tools;
using R3;
using UnityEngine.UIElements;
using VContainer;

namespace _StoryGame.Gameplay.UI.Impls.Viewer.Layers
{
    public sealed class HUDLayerHandler : UIViewerHandlerBase, IUIViewerLayerHandler
    {
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
            _fpsLabel = GetElement<Label>("fps");
        }

        protected override void Subscribe()
        {
            _fpsCounter.Fps.Subscribe(ShowFps).AddTo(Disposables);
        }

        protected override void Unsubscribe()
        {
        }

        private void ShowFps(float value) => _fpsLabel.text = value.ToString();

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
