using R3;

namespace _StoryGame.Core.Quest.Interfaces
{
    /// <summary>
    /// Интерфейс для целей квестов
    /// </summary>
    public interface IQuestObjective
    {
        /// <summary>
        /// Идентификатор цели
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Название цели
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Описание цели
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Тип цели
        /// </summary>
        ObjectiveType Type { get; }
        
        /// <summary>
        /// Статус цели
        /// </summary>
        ReactiveProperty<bool> IsCompleted { get; }
        
        /// <summary>
        /// Прогресс выполнения цели
        /// </summary>
        ReactiveProperty<float> Progress { get; }
        
        /// <summary>
        /// Целевое значение прогресса
        /// </summary>
        float TargetProgress { get; }
        
        /// <summary>
        /// Является ли цель обязательной
        /// </summary>
        bool IsRequired { get; }
        
        /// <summary>
        /// Цель, которая должна быть выполнена перед этой
        /// </summary>
        IQuestObjective Prerequisite { get; }
        
        /// <summary>
        /// Обновить прогресс цели
        /// </summary>
        /// <param name="progress">Прогресс</param>
        void UpdateProgress(float progress);
        
        /// <summary>
        /// Завершить цель
        /// </summary>
        void Complete();
        
        /// <summary>
        /// Сбросить прогресс цели
        /// </summary>
        void Reset();
        
        /// <summary>
        /// Проверить, доступна ли цель
        /// </summary>
        /// <returns>Доступность цели</returns>
        bool IsAvailable();
    }
    
    /// <summary>
    /// Типы целей
    /// </summary>
    public enum ObjectiveType
    {
        Collect,        // Собрать предметы
        Kill,           // Убить монстров
        Deliver,        // Доставить предметы
        Talk,           // Поговорить с NPC
        Explore,        // Исследовать область
        Defend,         // Защитить объект
        Cook,           // Приготовить блюдо
        Clean,          // Очистить область
        Trade,          // Торговать с NPC
        Escort,         // Сопроводить NPC
        Rescue,         // Спасти NPC
        Sabotage        // Саботировать объект
    }
} 