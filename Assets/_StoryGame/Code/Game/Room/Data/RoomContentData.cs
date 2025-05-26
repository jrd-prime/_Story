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
        public CoreNoteCurrencyData coreNote;
        public SpecialCurrencyData hidden;
        public InspectableLootData inspectableLoot;
    }

    [Serializable]
    public struct InspectableLootData
    {
        public List<CoreItemCurrencyData> coreItems;
        public List<NoteCurrencyData> notes;
        public int energy;
    }
}
