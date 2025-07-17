using _StoryGame.Core.Managers;
using _StoryGame.Core.Messaging.Interfaces;

namespace _StoryGame.Game.Managers.Condition.Messages
{
    public record GlobalConditionChangedMsg(EGlobalCondition GlobalCondition, bool NewValue)
        : IConditionRegistryMsg;
}
