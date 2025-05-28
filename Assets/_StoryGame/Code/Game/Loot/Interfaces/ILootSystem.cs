using System.Collections.Generic;
using _StoryGame.Game.Room;

namespace _StoryGame.Game.Loot
{
    public interface ILootSystem
    {
        GeneratedLootVo GetGeneratedLoot(string id);
        bool HasLoot(string id);
        bool GenerateLoot(IRoom room);
        List<LootType> GetLootFor(string roomId, string id);
    }
}
