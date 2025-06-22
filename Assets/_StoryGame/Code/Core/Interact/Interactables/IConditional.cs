using _StoryGame.Data.Interact;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Data.SO.Currency;

namespace _StoryGame.Core.Interact.Interactables
{
    public interface IConditional : IInteractable
    {
        SpecialItemData Loot { get; }
        ThoughtData LockedStateThought { get; }
        EConditionalState ConditionalState { get; }
        ACurrencyData[] ConditionalItems { get; }
        void SetConditionalState(EConditionalState state);
    }
}
