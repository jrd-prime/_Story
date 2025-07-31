namespace _StoryGame.Core.Interact.Interactables
{
    public interface IDynamicSwitchable : ISwitchable
    {
        bool InverseConditionImpact { get; }
        string NotFulfilledThoughtKey { get; }
    }
}
