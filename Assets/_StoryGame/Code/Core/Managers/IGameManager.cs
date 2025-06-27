using _StoryGame.Core.WalletNew.Interfaces;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Data.SO.Currency;
using R3;

namespace _StoryGame.Core.Managers
{
    /// <summary>
    /// Основной интерфейс для управления игровым процессом
    /// </summary>
    public interface IGameManager
    {
        IWallet TempWallet { get; }

        /// <summary>
        /// Текущее состояние игры
        /// </summary>
        ReactiveProperty<GameState> CurrentGameState { get; }

        /// <summary>
        /// Время игры
        /// </summary>
        ReactiveProperty<float> GameTime { get; }

        IWallet PlayerWallet { get; }

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

        bool IsPlayerHasItem(string itemId);
        bool IsPlayerHasConditionalItems(ACurrencyData[] conditionalItems);
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
