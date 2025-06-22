using _StoryGame.Data.Interact;
using _StoryGame.Data.Loot;
using _StoryGame.Data.Room;

namespace _StoryGame.Core.Room.Interfaces
{
    public interface IRoom
    {
        string Id { get; }
        string Name { get; }
        float Progress { get; }
        RoomLootVo Loot { get; }
        RoomInteractablesVo Interactables { get; }
        bool HasLoot(string inspectableId);
        InspectableLootVo GetLootData();
        InspectableData GetLoot(string inspectableId);
        bool UpdateStateForConditionalObjects();
    }
}
