using _StoryGame.Core.Loot;
using _StoryGame.Data.Interactable;
using _StoryGame.Data.Room;

namespace _StoryGame.Game.Interactables.Interfaces
{
    public interface IInspectable : IInteractable
    {
        EInspectState InspectState { get; }
        string Id { get; }
        RoomBaseLootChanceVo Chances { get; }
        void SetInspectState(EInspectState inspectState);
        int GetLootChance(ELootType eLootType);
    }
}
