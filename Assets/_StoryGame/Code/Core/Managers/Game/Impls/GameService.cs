using _StoryGame.Core.Interfaces.Managers;
using R3;

namespace _StoryGame.Core.Managers.Game.Impls
{
    public sealed class GameService : IGameService
    {
        public Observable<bool> IsGameStarted => _isGameStarted ? Observable.Return(true) : _onGameStartCommand;
        public ReactiveProperty<bool> IsGameRunning { get; } = new(false);
        public ReactiveProperty<bool> IsGamePaused { get; } = new(false);
        public ReactiveProperty<int> PlayerInitialHealth { get; } = new();

        private readonly HSM.Impls.HSM _hsm;
        private bool _isGameStarted;
        private readonly ReactiveCommand<bool> _onGameStartCommand = new();

        public GameService(HSM.Impls.HSM hsm) => _hsm = hsm;

        public void StartNewGame()
        {
            if (IsGameRunning.Value) return;

            _isGameStarted = true;
            IsGameRunning.Value = true;
            IsGamePaused.Value = false;
        }

        public void Pause()
        {
            if (!IsGameRunning.Value || IsGamePaused.Value) return;

            IsGameRunning.Value = false;
            IsGamePaused.Value = true;
        }

        public void UnPause()
        {
            if (!IsGamePaused.Value) return;

            IsGameRunning.Value = true;
            IsGamePaused.Value = false;
        }

        public void StopTheGame()
        {
            if (!IsGameRunning.Value) return;

            IsGameRunning.Value = false;
            IsGamePaused.Value = false;
        }

        public void GameOver()
        {
            IsGameRunning.Value = false;
            _isGameStarted = false;
        }

        public void ContinueGame()
        {
            // Логика продолжения, если нужна
        }

        public void StartHSM() => _hsm.Start();
    }
}
