namespace _StoryGame.Core.Managers.Game.Interfaces
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
