using _StoryGame.Core.Interact.Enums;
using _StoryGame.Game.Loot;

namespace _StoryGame.Core.Interact.Interactables
{
    public interface IInspectable : ILootable
    {
        EInspectState InspectState { get; }
        string Id { get; }
        void SetInspectState(EInspectState inspectState);
    }
}
