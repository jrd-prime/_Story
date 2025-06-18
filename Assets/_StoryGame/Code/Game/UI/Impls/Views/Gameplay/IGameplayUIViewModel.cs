using _StoryGame.Core.UI.Interfaces;
using R3;

namespace _StoryGame.Game.UI.Impls.Views.Gameplay
{
    public interface IGameplayUIViewModel : IUIViewModel
    {
        public Observable<float> FPS { get; }
    }
}
