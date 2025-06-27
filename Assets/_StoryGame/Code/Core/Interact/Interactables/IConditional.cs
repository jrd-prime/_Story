using _StoryGame.Core.Interact.Enums;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Data.SO.Currency;
using _StoryGame.Game.Loot;

namespace _StoryGame.Core.Interact.Interactables
{
    public interface IConditional :  ILootable
    {
        ThoughtData LockedStateThought { get; }
        EConditionalState ConditionalState { get; }
        ACurrencyData[] ConditionalItems { get; }
        void SetConditionalState(EConditionalState state);
        string GetSpecialItemId();
    }
}
