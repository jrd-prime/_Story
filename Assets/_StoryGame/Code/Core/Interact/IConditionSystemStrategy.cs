using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.Interactables.Condition;

namespace _StoryGame.Core.Interact
{
    public interface IConditionSystemStrategy : IInteractableSystemStrategy<IConditional>
    {
    }

    public interface IInspectSystemStrategy : IInteractableSystemStrategy<IInspectable>
    {
    }

    public interface IUseSystemStrategy : IInteractableSystemStrategy<IUsable>
    {
    }

    public interface IPassSystemStrategy : IInteractableSystemStrategy<IPassable>
    {
    }
}
