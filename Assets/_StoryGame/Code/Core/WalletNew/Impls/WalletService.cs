using System.Collections.Generic;
using _StoryGame.Core.WalletNew.Interfaces;

namespace _StoryGame.Core.WalletNew.Impls
{
    public sealed class WalletService : IWalletService
    {
        private readonly Dictionary<string, IWallet> _wallets = new();

        public IWallet GetOrCreate(string uid)
        {
            if (_wallets.TryGetValue(uid, out var wallet))
                return wallet;

            _wallets[uid] = new Wallet(uid);
            return _wallets[uid];
        }


        public bool Move(string fromWalletUid, string toWalletUid, string currencyId, long amount) => true;
        public bool Has(string uid, string currencyId) => true;
        public long Get(string uid, string currencyId) => 0;


        public bool Add(string uid, string currencyId, long amount)
        {
            var wallet = GetOrCreate(uid);

            return wallet.Add(currencyId, amount);
        }

        public bool Sub(string uid, string currencyId, long amount)
        {
            return true;
        }

        // public bool Add(string uid, LootDataVo[] loot)
        // {
        //     foreach (var item in loot)
        //     {
        //         if (!_currencyIcons.ContainsKey(item.currency.Id))
        //             _currencyIcons.Add(item.currency.Id, item.currency.Icon);
        //
        //         Add(uid, item.currency.Id, item.amount);
        //     }
        //
        //     return true;
        // }
        //
        // public bool Add(string uid, LootDataVo loot)
        // {
        //     if (!_currencyIcons.ContainsKey(loot.currency.Id))
        //         _currencyIcons.Add(loot.currency.Id, loot.currency.Icon);
        //
        //     Add(uid, loot.currency.Id, loot.amount);
        //     return true;
        // }
    }
}
