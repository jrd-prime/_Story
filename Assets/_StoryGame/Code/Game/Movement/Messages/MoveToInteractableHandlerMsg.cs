using _StoryGame.Core.Interfaces.Publisher.Messages;
using _StoryGame.Game.Interactables.Interfaces;

namespace _StoryGame.Game.Movement.Messages
{
    public record MoveToInteractableHandlerMsg(IInteractable Interactable) : IMovementHandlerMsg
    {
        public string Name => nameof(MoveToInteractableHandlerMsg);
        public IInteractable Interactable { get; } = Interactable;
    }
}
