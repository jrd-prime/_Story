using _StoryGame.Core.Messaging.Interfaces;

namespace _StoryGame.Game.Interact.Interactables.Unlock
{
    public record GlobalConditionChangedMsg(EGlobalInteractCondition GlobalCondition, bool NewValue)
        : IConditionRegistryMsg;
}
