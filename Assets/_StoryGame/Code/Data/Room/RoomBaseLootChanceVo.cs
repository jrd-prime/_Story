using System;
using _StoryGame.Core.Loot;

namespace _StoryGame.Data.Room
{
    [Serializable]
    public struct RoomBaseLootChanceVo
    {
        public int coreItemBaseChance;
        public int noteBaseChance;
        public int energyBaseChance;
        public int emptyBaseChance;

        public int GetLootChance(ELootType eLootType)
        {
            return eLootType switch
            {
                ELootType.Core => coreItemBaseChance,
                ELootType.Note => noteBaseChance,
                ELootType.Energy => energyBaseChance,
                _ => emptyBaseChance
            };
        }
    }
}
