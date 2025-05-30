using System.Collections.Generic;
using _StoryGame.Game.Room;

namespace _StoryGame.Game.Loot.Interfaces
{
    public interface ILootSystem
    {
        bool GenerateLoot(IRoom room);
        bool HasLoot(string roomId, string inspectableId);
        RoomLootData GetRoomLootData(string roomId);
        InspectableData GetLootForInspectable(string roomId, string inspectableId);
    }
}
