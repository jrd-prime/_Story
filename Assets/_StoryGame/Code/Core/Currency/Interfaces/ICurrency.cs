using _StoryGame.Core.Currency.Enums;

namespace _StoryGame.Core.Currency.Interfaces
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
        string Name { get; }

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
}
