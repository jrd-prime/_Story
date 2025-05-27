using System;
using System.Collections.Generic;
using _StoryGame.Data.Currency;
using _StoryGame.Game.Interactables.Types;

namespace _StoryGame.Game.Room.Data
{
    [Serializable]
    public struct RoomContentData
    {
        public RoomLootData loot;
        public RoomLootObjectsData lootObjects;
        public List<Usable> doors;
    }

    [Serializable]
    public struct RoomLootData
    {
        public CoreNoteCurrencyAData coreNote;
        public SpecialCurrencyAData hidden;
        public InspectableLootData inspectableLoot;
    }

    [Serializable]
    public struct InspectableLootData
    {
        public List<CoreItemCurrencyAData> coreItems;
        public List<NoteCurrencyAData> notes;
        public int energy;
    }
}
