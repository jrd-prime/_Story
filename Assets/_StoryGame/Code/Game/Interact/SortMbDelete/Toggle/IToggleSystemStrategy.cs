using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Game.Interact.SortMbDelete.InteractablesSORT;

namespace _StoryGame.Game.Interact.SortMbDelete.Toggle
{
    public interface IToggleSystemStrategy : IInteractableSystemStrategy<IToggleable>
    {
        void SetState(ESwitchState switchState);
    }
}
