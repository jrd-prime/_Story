using System;
using System.Collections.Generic;
using _StoryGame.Core.Currency.Interfaces;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Data.Loot;
using _StoryGame.Game.Interact.Interactables;

namespace _StoryGame.Game.Interact.Systems.Inspect.Strategies
{
    public sealed class LootGenerator
    {
        private readonly ILocalizationProvider _localizationProvider;
        private readonly ICurrencyRegistry _assetProvider;

        public LootGenerator(ILocalizationProvider localizationProvider, ICurrencyRegistry assetProvider)
        {
            _localizationProvider = localizationProvider;
            _assetProvider = assetProvider;
        }

        public PreparedObjLootData GenerateLootData(ILootable inspectable)
        {
            var loot1 = inspectable.Loot ??
                        throw new Exception("GenerateLootData - no loot data for " + inspectable.Name);
            var roomId = "ERR ROOM ID GenerateLootData";
            var inspectableId = inspectable.Id;
            var localizedName = _localizationProvider.Localize(inspectable.LocalizationKey, ETable.Words);


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
