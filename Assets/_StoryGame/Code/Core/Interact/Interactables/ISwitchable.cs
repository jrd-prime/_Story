using _StoryGame.Game.Interact.Interactables.Unlock;

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
