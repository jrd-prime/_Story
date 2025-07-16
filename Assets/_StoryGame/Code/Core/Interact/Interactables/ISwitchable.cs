using _StoryGame.Game.Interact.Interactables.Unlock;
using _StoryGame.Game.Managers.Condition;

namespace _StoryGame.Core.Interact.Interactables
{
    public interface ISwitchable : IInteractable
    {
        ConditionData ConditionsData { get; }
        EGlobalInteractCondition ImpactCondition { get; }
        void SwitchState();
        string GetSwitchInteractionQuestionKey();
    }
}
