using _StoryGame.Data.Interactable;
using _StoryGame.Data.Room;
using _StoryGame.Game.Loot.Impls;

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
    }
}
