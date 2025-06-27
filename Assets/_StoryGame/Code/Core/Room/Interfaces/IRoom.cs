using _StoryGame.Data.Room;
using UnityEngine;

namespace _StoryGame.Core.Room.Interfaces
{
    public interface IRoom
    {
        string Id { get; }
        string Name { get; }
        float Progress { get; }
        RoomLootVo Loot { get; }
        RoomInteractablesVo Interactables { get; }
        bool UpdateStateForConditionalObjects();
        Vector3 GetSpawnPosition();
    }
}
