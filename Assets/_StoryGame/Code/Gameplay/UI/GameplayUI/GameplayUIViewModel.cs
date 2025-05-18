using System;
using _StoryGame.Infrastructure.Bootstrap;
using _StoryGame.Infrastructure.Tools;
using R3;

namespace _StoryGame.Gameplay.UI.GameplayUI
{
    public interface IGameplayUIViewModel : IUIViewModel
    {
        public Observable<float> FPS { get; }
    }

    public class GameplayUIViewModel : IGameplayUIViewModel
    {
        public Observable<float> FPS => _fps;

        private readonly ReactiveProperty<float> _fps = new(0);
        private readonly CompositeDisposable _disposables = new();

        public GameplayUIViewModel(FPSCounter fps)
        {
            if (fps == null)
                throw new NullReferenceException("FPSCounter is null.");

            fps.Fps.Subscribe(value => _fps.Value = value).AddTo(_disposables);
        }
    }
}
