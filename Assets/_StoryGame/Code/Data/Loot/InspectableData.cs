using System.Collections.Generic;

namespace _StoryGame.Data.Loot
{
    public record InspectableData(string LocalizedName, List<InspectableLootData> InspectablesLoot);
}
