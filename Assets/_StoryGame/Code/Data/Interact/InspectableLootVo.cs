using System;
using _StoryGame.Data.Currency;

namespace _StoryGame.Data.Interact
{
    [Serializable]
    public struct InspectableLootVo
    {
        public CoreItemVo coreItem;
        public NotesVo notes;
        public EnergyVo energy;
    }
}
