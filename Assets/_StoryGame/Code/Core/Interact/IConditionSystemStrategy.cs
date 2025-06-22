namespace _StoryGame.Core.Interact
{
    public interface IConditionSystemStrategy : IInteractableSystemStrategy<IConditional>
    {
    }
    
    public interface IInspectSystemStrategy : IInteractableSystemStrategy<IInspectable>
    {
    }
}
