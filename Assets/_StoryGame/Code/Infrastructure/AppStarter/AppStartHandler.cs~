using R3;

namespace _StoryGame
{
    public sealed class AppStartHandler
    {
        public Subject<Unit> IsAppStarted { get; } = new();
        public void AppStarted() => IsAppStarted.OnNext(Unit.Default);
    }
}
