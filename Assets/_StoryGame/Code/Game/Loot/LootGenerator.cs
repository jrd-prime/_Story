using System;
using System.Collections.Generic;
using _StoryGame.Core.Currency.Interfaces;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Data.Loot;
using _StoryGame.Game.Interact.Interactables;

namespace _StoryGame.Game.Loot
{
    public sealed class LootGenerator
    {
        private readonly IL10nProvider _il10NProvider;
        private readonly ICurrencyRegistry _assetProvider;

        public LootGenerator(IL10nProvider il10NProvider, ICurrencyRegistry assetProvider)
        {
            _il10NProvider = il10NProvider;
            _assetProvider = assetProvider;
        }

        public PreparedObjLootData GenerateLootData(ILootable inspectable)
        {
            var loot1 = inspectable.Loot ??
                        throw new Exception("GenerateLootData - no loot data for " + inspectable.Name);
            var roomId = "ERR ROOM ID GenerateLootData";
            var inspectableId = inspectable.Id;
            var localizedName = _il10NProvider.Localize(inspectable.LocalizationKey, ETable.Words);


            List<PreparedLootVo> lootData = new();

            foreach (var loot in loot1)
            {
                var currency = loot.currency;
                var amount = loot.amount;
                var sprite = _assetProvider.GetIcon(currency.IconId);
                var info = new LootItemInfoVo(localizedName, roomId, inspectableId);
                var lootDataItem = new PreparedLootVo(currency, amount, sprite, info);
                lootData.Add(lootDataItem);
            }

            return new PreparedObjLootData(localizedName, lootData);
        }
    }

    public interface ILootable : IInteractable
    {
        OjectLootVo[] Loot { get; }
        bool HasLoot();
    }
}
