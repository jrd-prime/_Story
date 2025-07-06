using System.Collections.Generic;
using R3;

namespace _StoryGame.Core.WalletNew.Interfaces
{
    /// <summary>
    /// Интерфейс кошелька для объектов, взаимодействующих с валютой
    /// </summary>
    public interface IWallet
    {
        /// <summary>
        /// Уникальный идентификатор кошелька равен идентификатору оунера, который им владеет
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Получить текущее количество валюты по идентификатору
        /// </summary>
        /// <param name="currencyId">Идентификатор валюты</param>
        /// <returns>Количество валюты</returns>
        long Get(string currencyId);

        /// <summary>
        /// Добавить валюту в кошелек
        /// </summary>
        /// <param name="currencyId">Идентификатор валюты</param>
        /// <param name="amount">Количество</param>
        /// <returns>true, если операция выполнена успешно</returns>
        bool Add(string currencyId, long amount);

        bool Add(IReadOnlyDictionary<string, long> value);

        /// <summary>
        /// Удалить валюту из кошелька
        /// </summary>
        /// <param name="currencyId">Идентификатор валюты</param>
        /// <param name="amount">Количество</param>
        /// <returns>true, если операция выполнена успешно</returns>
        bool Sub(string currencyId, long amount);

        /// <summary>
        /// Проверить, достаточно ли валюты в кошельке
        /// </summary>
        /// <param name="currencyId">Идентификатор валюты</param>
        /// <param name="amount">Количество</param>
        /// <returns>true, если валюты достаточно</returns>
        bool Has(string currencyId, long amount);

        /// <summary>
        /// Получить словарь всех валют в кошельке
        /// </summary>
        /// <returns>Словарь валют (ключ - идентификатор валюты, значение - количество)</returns>
        IReadOnlyDictionary<string, long> GetAll();
        
        Observable<CurrencyChangedData> OnCurrencyChanged { get; }
    }
}
