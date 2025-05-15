using System;
using System.Collections.Generic;
using R3;

namespace _StoryGame.Core.Currency.Impls
{
    [Serializable]
    public sealed class Wallet : IWallet
    {
        public Subject<CurrencyChangeEvent> OnCurrencyChanged { get; }

        public string Id { get; }
        public string OwnerId { get; }

        private readonly Dictionary<string, int> _currencies = new();
        private readonly Subject<CurrencyChangeEvent> _currencyChangedSubject;

        public Wallet(string ownerId)
        {
            Id = ownerId + "_wallet";
            OwnerId = ownerId;
            _currencyChangedSubject = new Subject<CurrencyChangeEvent>();
            OnCurrencyChanged = _currencyChangedSubject;
        }

        public int GetCurrencyAmount(string currencyId) =>
            _currencies.ContainsKey(currencyId) ? _currencies[currencyId] : 0;

        public ReadOnlyReactiveProperty<int> GetCurrencyObservable(string currencyId)
        {
            return null;
        }

        public bool AddCurrency(string currencyId, int amount)
        {
            if (amount <= 0) return false;

            var previousAmount = GetCurrencyAmount(currencyId);
            var newAmount = previousAmount + amount;

            _currencies[currencyId] = newAmount;

            OnCurrencyAdded(currencyId, previousAmount, newAmount);
            return true;
        }

        private void OnCurrencyAdded(string currencyId, int previousAmount, int newAmount)
            => _currencyChangedSubject.OnNext(new CurrencyChangeEvent(currencyId, previousAmount, newAmount));

        public bool AddCurrency(IReadOnlyDictionary<string, int> value)
        {
            var result = true;
            foreach (var (id, amount) in value)
                if (!AddCurrency(id, amount))
                    result = false;

            return result;
        }

        public bool RemoveCurrency(string currencyId, int amount)
        {
            return false;
        }

        public bool HasEnoughCurrency(string currencyId, int amount) =>
            _currencies.TryGetValue(currencyId, out var amountInWallet) && amountInWallet >= amount;

        public IReadOnlyDictionary<string, int> GetAllCurrencies() => _currencies;
    }
}
