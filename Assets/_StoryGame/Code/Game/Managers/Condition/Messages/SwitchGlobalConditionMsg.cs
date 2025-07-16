using _StoryGame.Core.Managers;
using _StoryGame.Core.Messaging.Interfaces;

namespace _StoryGame.Game.Managers.Condition.Messages
{
    internal record SwitchGlobalConditionMsg(EGlobalInteractCondition GlobalCondition) : IConditionRegistryMsg;
}
