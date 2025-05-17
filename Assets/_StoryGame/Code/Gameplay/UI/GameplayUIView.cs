using _StoryGame.Data.UI;
using _StoryGame.Gameplay.Extensions;
using _StoryGame.Gameplay.UI.GameplayUI;
using _StoryGame.Infrastructure.Bootstrap;
using UnityEngine;
using UnityEngine.UIElements;
using R3;

namespace _StoryGame.Gameplay.UI
{
    public class GameplayUIView : UIView<IGameplayUIViewModel>
    {
        private VisualElement _ring;
        private Button _menuButton;

        private Label _fpsLabel;

        protected override void InitElements()
        {
            _ring = Root.Q<VisualElement>(UIConst.FullScreenRingIDName);
            _menuButton = Root.Q<Button>("menu-btn");
            _fpsLabel = Root.GetVisualElement<Label>("fps", name);
        }

        protected override void Subscribe()
        {
            ViewModel.FPS
                .Subscribe(OnFpsUpdate)
                .AddTo(Disposables);
        }

        private void OnFpsUpdate(float value) => _fpsLabel.text = value.ToString("0.0");

        private void SetRingPosition(Vector2 position)
        {
            _ring.style.left = position.x;
            _ring.style.top = position.y;
        }

        private void IsTouchPositionVisible(bool value)
        {
            _ring.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnDestroy() => Disposables.Dispose();
    }
}
