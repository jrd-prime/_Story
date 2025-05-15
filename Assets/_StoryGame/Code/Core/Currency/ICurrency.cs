namespace _StoryGame.Core.Currency
{
    /// <summary>
    /// Интерфейс, определяющий базовые свойства валюты
    /// </summary>
    public interface ICurrency
    {
        /// <summary>
        /// Уникальный идентификатор валюты
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Название валюты
        /// </summary>
        string LocalizationKey { get; }
        
        /// <summary>
        /// Описание валюты
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Иконка валюты
        /// </summary>
        string IconId { get; }
        
        /// <summary>
        /// Редкость валюты
        /// </summary>
        ECurrencyRarity Rarity { get; }
        
        /// <summary>
        /// Тип валюты
        /// </summary>
        ECurrencyType Type { get; }
        
        /// <summary>
        /// Максимальное количество в стаке
        /// </summary>
        int MaxStackSize { get; }
    }
    
    /// <summary>
    /// Перечисление типов валюты
    /// </summary>
    public enum ECurrencyType
    {
        /// <summary>
        /// Основная валюта (деньги)
        /// </summary>
        Money,
        
        /// <summary>
        /// Для готовки
        /// </summary>
        Ingredient,
        
        /// <summary>
        /// Для крафта
        /// </summary>
        Resource,
        
        /// <summary>
        /// Специальная валюта (очки репутации и т.д.)
        /// </summary>
        Special
    }
    
    /// <summary>
    /// Перечисление редкости валюты
    /// </summary>
    public enum ECurrencyRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
} 