using _StoryGame.Game.Interactables.Inspect;

namespace _StoryGame.Game.Interactables.Interfaces
{
    public interface IInspectable : IInteractable
    {
        EInspectState InspectState { get; }
        string Id { get; }
        void SetInspectState(EInspectState inspectState);
    }
}
