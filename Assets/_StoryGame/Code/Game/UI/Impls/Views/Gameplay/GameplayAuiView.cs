using _StoryGame.Infrastructure.Bootstrap;
using UnityEngine.UIElements;

namespace _StoryGame.Game.UI.Impls.Views.Gameplay
{
    public class GameplayAuiView : AuiView<IGameplayUIViewModel>
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
