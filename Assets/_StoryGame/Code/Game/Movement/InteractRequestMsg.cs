using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Messaging.Interfaces;

namespace _StoryGame.Game.Movement
{
    public record InteractRequestMsg(IInteractable Interactable) : IInteractProcessorMsg
    {
        public IInteractable Interactable { get; } = Interactable;
    }
}
