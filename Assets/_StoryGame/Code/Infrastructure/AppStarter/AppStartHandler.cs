using R3;

namespace _StoryGame.Infrastructure.AppStarter
{
    public sealed class AppStartHandler
    {
        public Subject<Unit> IsAppStarted { get; } = new();
        public void AppStarted() => IsAppStarted.OnNext(Unit.Default);
    }
}
