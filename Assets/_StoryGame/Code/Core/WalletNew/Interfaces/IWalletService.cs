namespace _StoryGame.Core.WalletNew.Interfaces
{
    public interface IWalletService
    {
        IWallet GetOrCreate(string uid);

        /// <summary>
        /// Получить информацию о количестве валюты в кошельке по идентификатору
        /// </summary>
        bool Has(string uid, string currencyId);

        /// <summary>
        /// Передать валюту между кошельками
        /// </summary>
        /// <returns>true, если операция выполнена успешно</returns>
        bool Move(string fromWalletUid, string toWalletUid, string currencyId, long amount);

        bool Add(string uid, string currencyId, long amount);
        bool Sub(string uid, string currencyId, long amount);
    }
}
