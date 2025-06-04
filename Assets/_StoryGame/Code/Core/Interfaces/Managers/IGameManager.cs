using _StoryGame.Core.WalletNew.Interfaces;

namespace _StoryGame.Core.Interfaces.Managers
{
    public interface IGameManager
    {
        IWallet TempWallet { get; }
        void GameOver();
        void StopTheGame();
        void StartNewGame();
        void Pause();
        void UnPause();
        void ContinueGame();
    }
}
