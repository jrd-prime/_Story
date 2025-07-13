using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Game.Interact.Interactables.Condition;

namespace _StoryGame.Game.Interact.Systems.Toggle
{
    public interface IToggleSystemStrategy : IInteractableSystemStrategy<IToggleable>
    {
        void SetState(ESwitchState switchState);
    }
}
