using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Loot;
using _StoryGame.Data.Room;
using _StoryGame.Game.Interact.Interactables;
using _StoryGame.Game.Interact.Systems.Inspect.Strategies;

namespace _StoryGame.Core.Interact.Interactables
{
    public interface IInspectable : ILootable
    {
        EInspectState InspectState { get; }
        string Id { get; }
        void SetInspectState(EInspectState inspectState);
    }
}
