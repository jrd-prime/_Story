using System;
using System.Collections.Generic;
using _StoryGame.Core.WalletNew.Interfaces;
using _StoryGame.Core.WalletNew.Messages;
using MessagePipe;
using R3;
using UnityEngine;

namespace _StoryGame.Core.WalletNew.Impls
{
    [Serializable]
    public sealed class Wallet : IWallet
    {
        public Observable<CurrencyChangedData> OnCurrencyChanged => _onCurrencyChangedSubject.AsObservable();
        public string Id { get; }

        private readonly Dictionary<string, long> _currencies = new();
        private readonly Subject<CurrencyChangedData> _onCurrencyChangedSubject = new();

        private readonly IPublisher<ItemAmountChangedMsg>
            _itemLootedMsgPub; // TODO ужасное решение, костыль, переделать

        public Wallet(string ownerId, IPublisher<ItemAmountChangedMsg> itemLootedMsgPub)
        {
            Id = ownerId;
            _itemLootedMsgPub = itemLootedMsgPub;
        }

        public long Get(string currencyId) => _currencies.ContainsKey(currencyId) ? _currencies[currencyId] : 0;

        public bool Add(string currencyId, long amount)
        {
            if (amount <= 0)
                return false;

            var previousAmount = Get(currencyId);
            var newAmount = previousAmount + amount;

            _currencies[currencyId] = newAmount;

            Debug.LogWarning("<color=green>" + Id + " / Add " + currencyId + " ... " + previousAmount + " -> " +
                             newAmount + "</color>");
            _onCurrencyChangedSubject.OnNext(new CurrencyChangedData(currencyId, previousAmount, newAmount));

            var currentAmount = Get(currencyId);
            _itemLootedMsgPub.Publish(new ItemAmountChangedMsg(currencyId, currentAmount));
            return true;
        }

        public bool Add(IReadOnlyDictionary<string, long> value)
        {
            var result = true;
            foreach (var (id, amount) in value)
                if (!Add(id, amount))
                    result = false;

            return result;
        }

        public bool Sub(string currencyId, long amount)
        {
            if (Has(currencyId, amount))
            {
                var previousAmount = Get(currencyId);
                var newAmount = previousAmount - amount;

                _currencies[currencyId] = newAmount;

                Debug.LogWarning("Sub " + currencyId + " ... " + previousAmount + " -> " + newAmount);

                _onCurrencyChangedSubject.OnNext(new CurrencyChangedData(currencyId, previousAmount, newAmount));

                Debug.LogWarning("Sub " + currencyId + " ... " + amount);
                var currentAmount = Get(currencyId);
                _itemLootedMsgPub.Publish(new ItemAmountChangedMsg(currencyId, currentAmount));
                return true;
            }

            return false;
        }

        public bool Has(string currencyId, long amount) =>
            _currencies.TryGetValue(currencyId, out var amountInWallet) && amountInWallet >= amount;

        public IReadOnlyDictionary<string, long> GetAll() => _currencies;
    }
}
