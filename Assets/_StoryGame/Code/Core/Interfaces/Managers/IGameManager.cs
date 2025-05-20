namespace _StoryGame.Core.Interfaces.Managers
{
    public interface IGameManager 
    {
        void GameOver();
        void StopTheGame();
        void StartNewGame();
        void Pause();
        void UnPause();
        void ContinueGame();
    }
}
