using _StoryGame.Core.Interact.Interactables;

namespace _StoryGame.Game.Interact.Interactables.Unlock
{
    public interface IUnlockable : IInteractable
    {
        EUnlockableState UnlockableState { get; }
    }
}
