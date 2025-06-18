using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Data.Loot;

namespace _StoryGame.Core.Loot.Interfaces
{
    public interface ILootSystem
    {
        bool GenerateLoot(IRoom room);
        bool HasLoot(string roomId, string inspectableId);
        RoomLootData GetRoomLootData(string roomId);
        InspectableData GetLootForInspectable(string roomId, string inspectableId);
    }
}
