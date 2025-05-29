using _StoryGame.Game.Room;

namespace _StoryGame.Game.Loot.Interfaces
{
    public interface ILootSystem
    {
        bool HasLoot(string roomId, string inspectableId);
        bool GenerateLoot(IRoom room);
        GeneratedLootForInspectableVo GetGeneratedLoot(string roomId, string inspectableId);
    }
}
