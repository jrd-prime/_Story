using System.Collections.Generic;
using _StoryGame.Core.Loot;

namespace _StoryGame.Data.Loot
{
    /// <summary>
    /// Содержит список типов лута для какждого исследуемого объекта по Id
    /// </summary>
    public record GeneratedRoomLootTypesData(Dictionary<string, List<ELootType>> Loot)
    {
        public Dictionary<string, List<ELootType>> Loot { get; } = Loot; // <inspectable id, loot types>
    }
}
