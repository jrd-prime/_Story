using System.Collections.Generic;
using _StoryGame.Core.WalletNew.Interfaces;
using _StoryGame.Core.WalletNew.Messages;
using MessagePipe;

namespace _StoryGame.Core.WalletNew.Impls
{
    public sealed class WalletService : IWalletService
    {
        private readonly Dictionary<string, IWallet> _wallets = new();
        private readonly IPublisher<ItemAmountChangedMsg> _itemLootedMsgPub;

        public WalletService(IPublisher<ItemAmountChangedMsg> itemLootedMsgPub)
        {
            _itemLootedMsgPub = itemLootedMsgPub;
        }

        public IWallet GetOrCreate(string uid)
        {
            if (_wallets.TryGetValue(uid, out var wallet))
                return wallet;

            _wallets[uid] = new Wallet(uid, _itemLootedMsgPub);
            return _wallets[uid];
        }


        public bool Move(string fromWalletUid, string toWalletUid, string currencyId, long amount) => true;
        public bool Has(string uid, string currencyId) => true;
        public long Get(string uid, string currencyId) => 0;


        public bool Add(string uid, string currencyId, long amount)
        {
            var wallet = GetOrCreate(uid);

            if (!wallet.Add(currencyId, amount))
                return false;
            return true;
        }

        public bool Sub(string uid, string currencyId, long amount)
        {
            var wallet = GetOrCreate(uid);

            if (!wallet.Sub(currencyId, amount))
                return false;

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
