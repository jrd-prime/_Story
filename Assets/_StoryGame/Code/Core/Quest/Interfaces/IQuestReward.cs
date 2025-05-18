using _StoryGame.Core.Character.Player.Interfaces;

namespace _StoryGame.Core.Quest.Interfaces
{
    /// <summary>
    /// Интерфейс для наград за квесты
    /// </summary>
    public interface IQuestReward
    {
        /// <summary>
        /// Идентификатор награды
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Название награды
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Описание награды
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Тип награды
        /// </summary>
        RewardType Type { get; }
        
        /// <summary>
        /// Количество награды
        /// </summary>
        int Amount { get; }
        
        /// <summary>
        /// Редкость награды
        /// </summary>
        RewardRarity Rarity { get; }
        
        /// <summary>
        /// Идентификатор предмета (если награда - предмет)
        /// </summary>
        string ItemId { get; }
        
        /// <summary>
        /// Применить награду к игроку
        /// </summary>
        /// <param name="player">Игрок</param>
        void Apply(IPlayer player);
        
        /// <summary>
        /// Получить стоимость награды
        /// </summary>
        /// <returns>Стоимость</returns>
        int GetValue();
    }
    
    /// <summary>
    /// Типы наград
    /// </summary>
    public enum RewardType
    {
        Money,          // Деньги
        Experience,     // Опыт
        Reputation,     // Репутация
        Item,           // Предмет
        Recipe,         // Рецепт
        Ingredient,     // Ингредиент
        Room,           // Комната
        Furniture,      // Мебель
        Decoration,     // Декорация
        Ability,        // Способность
        Companion,      // Компаньон
        Special         // Специальная награда
    }
    
    /// <summary>
    /// Редкость наград
    /// </summary>
    public enum RewardRarity
    {
        Common,         // Обычная
        Uncommon,       // Необычная
        Rare,           // Редкая
        VeryRare,       // Очень редкая
        Legendary,      // Легендарная
        Mythical        // Мифическая
    }
} 