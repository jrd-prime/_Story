using System.Collections.Generic;
using _StoryGame.Core.Character.Common.Interfaces;
using R3;

namespace _StoryGame.Core.Quest.Interfaces
{
    /// <summary>
    /// Интерфейс для квестов
    /// </summary>
    public interface IQuest
    {
        /// <summary>
        /// Идентификатор квеста
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Название квеста
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Описание квеста
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Тип квеста
        /// </summary>
        QuestType Type { get; }
        
        /// <summary>
        /// Сложность квеста
        /// </summary>
        QuestDifficulty Difficulty { get; }
        
        /// <summary>
        /// Статус квеста
        /// </summary>
        ReactiveProperty<QuestStatus> Status { get; }
        
        /// <summary>
        /// Прогресс выполнения квеста
        /// </summary>
        ReactiveProperty<float> Progress { get; }
        
        /// <summary>
        /// Время начала квеста
        /// </summary>
        float StartTime { get; }
        
        /// <summary>
        /// Время завершения квеста
        /// </summary>
        float EndTime { get; }
        
        /// <summary>
        /// Ограничение по времени (в секундах)
        /// </summary>
        float TimeLimit { get; }
        
        /// <summary>
        /// Список целей квеста
        /// </summary>
        List<IQuestObjective> Objectives { get; }
        
        /// <summary>
        /// Список наград за квест
        /// </summary>
        List<IQuestReward> Rewards { get; }
        
        /// <summary>
        /// Требуемый уровень игрока
        /// </summary>
        int RequiredPlayerLevel { get; }
        
        /// <summary>
        /// Требуемая репутация таверны
        /// </summary>
        float RequiredTavernReputation { get; }
        
        /// <summary>
        /// Персонаж, выдавший квест
        /// </summary>
        ICharacter QuestGiver { get; }
        
        /// <summary>
        /// Начать квест
        /// </summary>
        void Start();
        
        /// <summary>
        /// Обновить прогресс квеста
        /// </summary>
        /// <param name="progress">Прогресс</param>
        void UpdateProgress(float progress);
        
        /// <summary>
        /// Обновить статус цели
        /// </summary>
        /// <param name="objectiveId">Идентификатор цели</param>
        /// <param name="completed">Завершена ли цель</param>
        void UpdateObjectiveStatus(string objectiveId, bool completed);
        
        /// <summary>
        /// Завершить квест
        /// </summary>
        void Complete();
        
        /// <summary>
        /// Провалить квест
        /// </summary>
        void Fail();
        
        /// <summary>
        /// Отменить квест
        /// </summary>
        void Cancel();
        
        /// <summary>
        /// Получить награды за квест
        /// </summary>
        /// <returns>Список наград</returns>
        List<IQuestReward> GetRewards();
        
        /// <summary>
        /// Проверить, доступен ли квест для игрока
        /// </summary>
        /// <param name="playerLevel">Уровень игрока</param>
        /// <param name="tavernReputation">Репутация таверны</param>
        /// <returns>Доступность квеста</returns>
        bool IsAvailableForPlayer(int playerLevel, float tavernReputation);
    }
    
    /// <summary>
    /// Типы квестов
    /// </summary>
    public enum QuestType
    {
        Gathering,      // Сбор ингредиентов
        Hunting,        // Охота на монстров
        Delivery,       // Доставка предметов
        Escort,         // Сопровождение
        Exploration,    // Исследование
        Defense,        // Защита
        Cooking,        // Приготовление
        Cleaning,       // Уборка
        Trading,        // Торговля
        Diplomacy,      // Дипломатия
        Rescue,         // Спасение
        Sabotage        // Саботаж
    }
    
    /// <summary>
    /// Сложность квестов
    /// </summary>
    public enum QuestDifficulty
    {
        VeryEasy,       // Очень легкий
        Easy,           // Легкий
        Medium,         // Средний
        Hard,           // Сложный
        VeryHard,       // Очень сложный
        Extreme         // Экстремальный
    }
    
    /// <summary>
    /// Статусы квестов
    /// </summary>
    public enum QuestStatus
    {
        Available,      // Доступен
        Active,         // Активен
        Completed,      // Завершен
        Failed,         // Провален
        Cancelled       // Отменен
    }
} 