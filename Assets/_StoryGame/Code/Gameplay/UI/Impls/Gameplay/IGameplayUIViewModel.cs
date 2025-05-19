using _StoryGame.Infrastructure.Bootstrap;
using R3;

namespace _StoryGame.Gameplay.UI.Impls.Gameplay
{
    public interface IGameplayUIViewModel : IUIViewModel
    {
        public Observable<float> FPS { get; }
    }
}
