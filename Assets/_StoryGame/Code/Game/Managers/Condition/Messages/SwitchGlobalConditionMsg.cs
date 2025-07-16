using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Game.Interact.Interactables.Unlock;

namespace _StoryGame.Game.Managers.Condition.Messages
{
    internal record SwitchGlobalConditionMsg(EGlobalInteractCondition GlobalCondition) : IConditionRegistryMsg;
}
