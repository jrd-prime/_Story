using _StoryGame.Infrastructure.Bootstrap;
using R3;

namespace _StoryGame.Game.UI.Impls.Views.Gameplay
{
    public interface IGameplayUIViewModel : IUIViewModel
    {
        public Observable<float> FPS { get; }
    }
}
