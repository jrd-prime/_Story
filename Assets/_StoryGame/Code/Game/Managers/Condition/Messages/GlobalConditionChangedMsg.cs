using _StoryGame.Core.Managers;
using _StoryGame.Core.Messaging.Interfaces;

namespace _StoryGame.Game.Managers.Condition.Messages
{
    public record GlobalConditionChangedMsg(EGlobalInteractCondition GlobalCondition, bool NewValue)
        : IConditionRegistryMsg;
}
