using _StoryGame.Infrastructure.Bootstrap;
using R3;

namespace _StoryGame.Game.UI.Impls.Gameplay
{
    public interface IGameplayUIViewModel : IUIViewModel
    {
        public Observable<float> FPS { get; }
    }
}
