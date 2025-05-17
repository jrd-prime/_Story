using System.Collections.Generic;
using _StoryGame.Core.Extensions;

namespace _StoryGame.Core.Currency.Impls
{
    public sealed class CurrencyService : ICurrencyService
    {
        public ICurrency GetCurrencyInfo(string currencyId) => null;
        public IReadOnlyList<ICurrency> GetAllCurrencies() => null;
        public IReadOnlyList<ICurrency> GetCurrenciesByType(ECurrencyType type) => null;
        public IReadOnlyList<ICurrency> GetCurrenciesByRarity(ECurrencyRarity rarity) => null;

        private readonly Dictionary<string, IWallet> _wallets = new();

        public IWallet CreateWallet(string ownerId)
        {
            var wallet = _wallets.TryGetValue(ownerId, out var result) ? result : new Wallet(ownerId);

            wallet.CheckOnNull(nameof(CurrencyService));

            _wallets[ownerId] = wallet;
            return wallet;
        }

        public IWallet GetWallet(string walletId)
        {
            if (_wallets.TryGetValue(walletId, out var wallet)) return wallet;

            // Log.Error($"Wallet with id {walletId} not found. Create new wallet? Can return null???");
            return null;
        }

        public IWallet GetWalletByOwner(string ownerId)
        {
            if (_wallets.TryGetValue(ownerId, out var wallet)) return wallet;

            // Log.Error($"Wallet with ownerId {ownerId} not found. Create new wallet? Can return null???");
            return null;
        }

        public bool TransferCurrency(string fromWalletId, string toWalletId, string currencyId, int amount) => false;
        public int ConvertCurrency(string walletId, string fromCurrencyId, string toCurrencyId, int amount) => 0;
        public float GetConversionRate(string fromCurrencyId, string toCurrencyId) => 0;
    }
}
