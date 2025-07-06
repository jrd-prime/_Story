using System.Collections.Generic;

namespace _StoryGame.Data.Loot
{
    public record RoomLootData(Dictionary<string, PreparedObjLootData> InspectableData)
    {
        /// <summary>
        /// InteractableId -> PreparedObjLootData  
        /// </summary>
        public Dictionary<string, PreparedObjLootData> InspectableData { get; } = InspectableData;
    }
}
