using System.Collections.Generic;
using _StoryGame.Game.Interactables.Interfaces;
using _StoryGame.Game.Room;

namespace _StoryGame.Game.Loot.Interfaces
{
    public interface ILootSystem
    {
        GeneratedLootVo GetGeneratedLoot(IInspectable inspectable);
        bool HasLoot(string roomId, string inspectableId);
        bool GenerateLoot(IRoom room);
        List<LootType> GetLootFor(string roomId, string id);
    }
}
