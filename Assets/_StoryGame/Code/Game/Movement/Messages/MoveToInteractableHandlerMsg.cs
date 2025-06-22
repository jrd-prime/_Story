using _StoryGame.Core.Interact;
using _StoryGame.Core.Messaging.Interfaces;

namespace _StoryGame.Game.Movement.Messages
{
    public record MoveToInteractableHandlerMsg(IInteractable Interactable) : IMovementHandlerMsg
    {
        public string Name => nameof(MoveToInteractableHandlerMsg);
        public IInteractable Interactable { get; } = Interactable;
    }
}
