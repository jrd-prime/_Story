using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.Interactables.Unlock;

namespace _StoryGame.Game.Interact.Interactables.Condition
{
    public interface IToggleable : IInteractable
    {
        ConditionData ConditionsData { get; }
        EToggleType ToggleType { get; }
        ESwitchState SwitchState { get; }
    }
}
