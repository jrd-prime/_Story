using System.Collections.Generic;

namespace _StoryGame.Data.Loot
{
    public record RoomLootData(Dictionary<string, InspectableData> InspectableData)
    {
        /// <summary>
        /// InspectableId -> InspectableData  
        /// </summary>
        public Dictionary<string, InspectableData> InspectableData { get; } = InspectableData;
    }
}
