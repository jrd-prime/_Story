using R3;

namespace _StoryGame.Core.Interfaces
{
    /// <summary>
    /// Основной интерфейс для управления игровым процессом
    /// </summary>
    public interface IGameManager
    {
        /// <summary>
        /// Текущее состояние игры
        /// </summary>
        ReactiveProperty<GameState> CurrentGameState { get; }
        
        /// <summary>
        /// Время игры
        /// </summary>
        ReactiveProperty<float> GameTime { get; }
        
        /// <summary>
        /// Запуск игры
        /// </summary>
        void StartGame();
        
        /// <summary>
        /// Пауза игры
        /// </summary>
        void PauseGame();
        
        /// <summary>
        /// Продолжение игры
        /// </summary>
        void ResumeGame();
        
        /// <summary>
        /// Завершение игры
        /// </summary>
        void EndGame();
        
        /// <summary>
        /// Сохранение игры
        /// </summary>
        void SaveGame();
        
        /// <summary>
        /// Загрузка игры
        /// </summary>
        void LoadGame();
    }
    
    /// <summary>
    /// Перечисление состояний игры
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Loading,
        Playing,
        Paused,
        Inventory,
        Dialog,
        Quest,
        GameOver
    }
} 