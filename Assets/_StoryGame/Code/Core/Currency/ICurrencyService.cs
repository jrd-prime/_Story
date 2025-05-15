using System.Collections.Generic;

namespace _StoryGame.Core.Currency
{
    /// <summary>
    /// Интерфейс сервиса управления валютой
    /// </summary>
    public interface ICurrencyService
    {
        /// <summary>
        /// Получить информацию о валюте по идентификатору
        /// </summary>
        /// <param name="currencyId">Идентификатор валюты</param>
        /// <returns>Информация о валюте</returns>
        ICurrency GetCurrencyInfo(string currencyId);
        
        /// <summary>
        /// Получить список всех доступных валют
        /// </summary>
        /// <returns>Список валют</returns>
        IReadOnlyList<ICurrency> GetAllCurrencies();
        
        /// <summary>
        /// Получить список валют по типу
        /// </summary>
        /// <param name="type">Тип валюты</param>
        /// <returns>Список валют указанного типа</returns>
        IReadOnlyList<ICurrency> GetCurrenciesByType(ECurrencyType type);
        
        /// <summary>
        /// Получить список валют по редкости
        /// </summary>
        /// <param name="rarity">Редкость валюты</param>
        /// <returns>Список валют указанной редкости</returns>
        IReadOnlyList<ICurrency> GetCurrenciesByRarity(ECurrencyRarity rarity);
        
        /// <summary>
        /// Создать новый кошелек
        /// </summary>
        /// <param name="ownerId">Идентификатор владельца</param>
        /// <returns>Новый кошелек</returns>
        IWallet CreateWallet(string ownerId);
        
        /// <summary>
        /// Получить кошелек по идентификатору
        /// </summary>
        /// <param name="walletId">Идентификатор кошелька</param>
        /// <returns>Кошелек или null, если не найден</returns>
        IWallet GetWallet(string walletId);
        
        /// <summary>
        /// Получить кошелек по идентификатору владельца
        /// </summary>
        /// <param name="ownerId">Идентификатор владельца</param>
        /// <returns>Кошелек или null, если не найден</returns>
        IWallet GetWalletByOwner(string ownerId);
        
        /// <summary>
        /// Передать валюту между кошельками
        /// </summary>
        /// <param name="fromWalletId">Идентификатор кошелька отправителя</param>
        /// <param name="toWalletId">Идентификатор кошелька получателя</param>
        /// <param name="currencyId">Идентификатор валюты</param>
        /// <param name="amount">Количество</param>
        /// <returns>true, если операция выполнена успешно</returns>
        bool TransferCurrency(string fromWalletId, string toWalletId, string currencyId, int amount);
        
        /// <summary>
        /// Конвертировать одну валюту в другую
        /// </summary>
        /// <param name="walletId">Идентификатор кошелька</param>
        /// <param name="fromCurrencyId">Идентификатор исходной валюты</param>
        /// <param name="toCurrencyId">Идентификатор целевой валюты</param>
        /// <param name="amount">Количество исходной валюты</param>
        /// <returns>Количество полученной валюты или 0, если конвертация не удалась</returns>
        int ConvertCurrency(string walletId, string fromCurrencyId, string toCurrencyId, int amount);
        
        /// <summary>
        /// Получить коэффициент конвертации между валютами
        /// </summary>
        /// <param name="fromCurrencyId">Идентификатор исходной валюты</param>
        /// <param name="toCurrencyId">Идентификатор целевой валюты</param>
        /// <returns>Коэффициент конвертации (сколько единиц целевой валюты за 1 единицу исходной)</returns>
        float GetConversionRate(string fromCurrencyId, string toCurrencyId);
    }
} 