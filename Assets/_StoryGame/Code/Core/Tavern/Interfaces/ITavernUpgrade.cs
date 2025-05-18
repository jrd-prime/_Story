using System.Collections.Generic;

namespace _StoryGame.Core.Tavern.Interfaces
{
    /// <summary>
    /// Интерфейс для улучшений таверны
    /// </summary>
    public interface ITavernUpgrade
    {
        /// <summary>
        /// Идентификатор улучшения
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Название улучшения
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Описание улучшения
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Тип улучшения
        /// </summary>
        UpgradeType Type { get; }
        
        /// <summary>
        /// Стоимость улучшения
        /// </summary>
        int Cost { get; }
        
        /// <summary>
        /// Требуемый уровень таверны
        /// </summary>
        int RequiredTavernLevel { get; }
        
        /// <summary>
        /// Требуемая репутация таверны
        /// </summary>
        float RequiredReputation { get; }
        
        /// <summary>
        /// Требуемые предыдущие улучшения
        /// </summary>
        List<string> RequiredUpgrades { get; }
        
        /// <summary>
        /// Получить бонусы улучшения
        /// </summary>
        /// <returns>Словарь бонусов</returns>
        Dictionary<BonusType, float> GetBonuses();
        
        /// <summary>
        /// Применить улучшение к таверне
        /// </summary>
        /// <param name="tavernManager">Менеджер таверны</param>
        void Apply(ITavernManager tavernManager);
    }
    
    /// <summary>
    /// Типы улучшений
    /// </summary>
    public enum UpgradeType
    {
        RoomExpansion,      // Расширение комнаты
        KitchenUpgrade,     // Улучшение кухни
        AtmosphereUpgrade,  // Улучшение атмосферы
        SecurityUpgrade,    // Улучшение безопасности
        StorageUpgrade,     // Улучшение хранилища
        StaffUpgrade,       // Улучшение персонала
        ReputationBoost,    // Усиление репутации
        SpecialAbility      // Специальная способность
    }
} 