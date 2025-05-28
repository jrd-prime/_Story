using System;
using _StoryGame.Game.Loot;

namespace _StoryGame.Game.Interactables.Impls
{
    [Serializable]
    public struct RoomBaseLootChanceVo
    {
        public int coreItemBaseChance;
        public int noteBaseChance;
        public int energyBaseChance;
        public int emptyBaseChance;

        public int GetLootChance(LootType lootType)
        {
            return lootType switch
            {
                LootType.Core => coreItemBaseChance,
                LootType.Note => noteBaseChance,
                LootType.Energy => energyBaseChance,
                _ => emptyBaseChance
            };
        }
    }
}
