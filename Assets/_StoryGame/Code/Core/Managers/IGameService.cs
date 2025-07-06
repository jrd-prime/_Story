using R3;

namespace _StoryGame.Core.Managers
{
    public interface IGameService
    {
        Observable<bool> IsGameStarted { get; }
        ReactiveProperty<bool> IsGameRunning { get; }
        ReactiveProperty<bool> IsGamePaused { get; }
        ReactiveProperty<int> PlayerInitialHealth { get; }

        void StartNewGame();
        void Pause();
        void UnPause();
        void StopTheGame();
        void GameOver();
        void ContinueGame();
        void StartHSM();
    }
}
