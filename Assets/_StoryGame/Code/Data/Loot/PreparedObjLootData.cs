using System.Collections.Generic;

namespace _StoryGame.Data.Loot
{
    public record PreparedObjLootData(string LocalizedName, List<PreparedLootVo> InspectablesLoot);
}
