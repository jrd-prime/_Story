using System.Collections.Generic;
using R3;

namespace _StoryGame.Core.Currency
{
    /// <summary>
    /// Интерфейс кошелька для объектов, взаимодействующих с валютой
    /// </summary>
    public interface IWallet
    {
        /// <summary>
        /// Уникальный идентификатор кошелька
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Владелец кошелька
        /// </summary>
        string OwnerId { get; }

        /// <summary>
        /// Получить текущее количество валюты по идентификатору
        /// </summary>
        /// <param name="currencyId">Идентификатор валюты</param>
        /// <returns>Количество валюты</returns>
        int GetCurrencyAmount(string currencyId);

        /// <summary>
        /// Получить реактивное свойство для отслеживания изменений количества валюты
        /// </summary>
        /// <param name="currencyId">Идентификатор валюты</param>
        /// <returns>Реактивное свойство с текущим количеством валюты</returns>
        ReadOnlyReactiveProperty<int> GetCurrencyObservable(string currencyId);

        /// <summary>
        /// Добавить валюту в кошелек
        /// </summary>
        /// <param name="currencyId">Идентификатор валюты</param>
        /// <param name="amount">Количество</param>
        /// <returns>true, если операция выполнена успешно</returns>
        bool AddCurrency(string currencyId, int amount);

        bool AddCurrency(IReadOnlyDictionary<string, int> value);

        /// <summary>
        /// Удалить валюту из кошелька
        /// </summary>
        /// <param name="currencyId">Идентификатор валюты</param>
        /// <param name="amount">Количество</param>
        /// <returns>true, если операция выполнена успешно</returns>
        bool RemoveCurrency(string currencyId, int amount);

        /// <summary>
        /// Проверить, достаточно ли валюты в кошельке
        /// </summary>
        /// <param name="currencyId">Идентификатор валюты</param>
        /// <param name="amount">Количество</param>
        /// <returns>true, если валюты достаточно</returns>
        bool HasEnoughCurrency(string currencyId, int amount);

        /// <summary>
        /// Получить словарь всех валют в кошельке
        /// </summary>
        /// <returns>Словарь валют (ключ - идентификатор валюты, значение - количество)</returns>
        IReadOnlyDictionary<string, int> GetAllCurrencies();

        /// <summary>
        /// Событие изменения количества валюты
        /// </summary>
        Subject<CurrencyChangeEvent> OnCurrencyChanged { get; }
    }

    /// <summary>
    /// Класс события изменения валюты
    /// </summary>
    public class CurrencyChangeEvent
    {
        /// <summary>
        /// Идентификатор валюты
        /// </summary>
        public string CurrencyId { get; }

        /// <summary>
        /// Предыдущее количество
        /// </summary>
        public int PreviousAmount { get; }

        /// <summary>
        /// Новое количество
        /// </summary>
        public int NewAmount { get; }

        /// <summary>
        /// Разница (может быть положительной или отрицательной)
        /// </summary>
        public int Delta => NewAmount - PreviousAmount;

        public CurrencyChangeEvent(string currencyId, int previousAmount, int newAmount)
        {
            CurrencyId = currencyId;
            PreviousAmount = previousAmount;
            NewAmount = newAmount;
        }
    }
}
