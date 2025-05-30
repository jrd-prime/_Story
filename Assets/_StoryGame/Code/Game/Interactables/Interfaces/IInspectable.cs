using _StoryGame.Core.Loot;
using _StoryGame.Data.Room;
using _StoryGame.Game.Interactables.Impls.Inspect;

namespace _StoryGame.Game.Interactables.Interfaces
{
    public interface IInspectable : IInteractable
    {
        EInspectState InspectState { get; }
        string Id { get; }
        RoomBaseLootChanceVo Chances { get; }
        void SetInspectState(EInspectState inspectState);
        int GetLootChance(LootType lootType);
    }
}
