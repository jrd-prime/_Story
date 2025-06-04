using System.Collections.Generic;
using _StoryGame.Core.Currency.Enums;
using _StoryGame.Infrastructure.Bootstrap.Interfaces;
using UnityEngine;

namespace _StoryGame.Core.Currency.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса управления валютой. Знает всё про валюту
    /// </summary>
    public interface ICurrencyRegistry : IBootable
    {
        /// <summary>
        /// Получить информацию о валюте по идентификатору
        /// </summary>
        ICurrency Get(string uid, string currencyId);

        /// <summary>
        /// Получить список всех доступных валют
        /// </summary>
        /// <returns>Список валют</returns>
        IReadOnlyList<ICurrency> GetAll();

        /// <summary>
        /// Получить список валют по типу
        /// </summary>
        /// <returns>Список валют указанного типа</returns>
        IReadOnlyList<ICurrency> GetCurrenciesByType(ECurrencyType type);

        /// <summary>
        /// Получить список валют по редкости
        /// </summary>
        /// <returns>Список валют указанной редкости</returns>
        IReadOnlyList<ICurrency> GetCurrenciesByRarity(ECurrencyRarity rarity);

        Sprite GetIcon(string currencyId);


        /// <summary>
        /// Конвертировать одну валюту в другую
        /// </summary>
        /// <returns>Количество полученной валюты или 0, если конвертация не удалась</returns>
        int ConvertCurrency(string walletUid, string fromCurrencyId, string toCurrencyId, long amount);

        /// <summary>
        /// Получить коэффициент конвертации между валютами
        /// </summary>
        /// <returns>Коэффициент конвертации (сколько единиц целевой валюты за 1 единицу исходной)</returns>
        float GetConversionRate(string fromCurrencyId, string toCurrencyId);
    }
}
