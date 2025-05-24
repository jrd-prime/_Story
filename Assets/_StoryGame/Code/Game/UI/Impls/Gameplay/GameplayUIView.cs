using _StoryGame.Infrastructure.Bootstrap;
using UnityEngine.UIElements;

namespace _StoryGame.Game.UI.Impls.Gameplay
{
    public class GameplayUIView : UIView<IGameplayUIViewModel>
    {
        private Button _menuButton;

        protected override void InitElements()
        {
            _menuButton = Root.Q<Button>("menu-btn");
        }

        protected override void Subscribe()
        {
        }

        private void OnDestroy() => Disposables.Dispose();
    }
}
