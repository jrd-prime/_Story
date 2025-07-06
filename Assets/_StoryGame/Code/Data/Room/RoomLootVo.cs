using System;
using _StoryGame.Data.Interact;
using _StoryGame.Data.SO.Currency;

namespace _StoryGame.Data.Room
{
    [Serializable]
    public struct RoomLootVo
    {
        public CoreNoteData coreNoteData;
        public SpecialItemData hidden;
        public InspectableLootVo inspectableLoot;
        public InspectableLootVo GetLootObjects() => inspectableLoot;
    }
}
