using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Game.Interact.Interactables.Unlock;

namespace _StoryGame.Game.Interact.InteractableNew.Conditional.Switchable.Impl
{
    internal record SwitchGlobalConditionMsg(EGlobalInteractCondition GlobalCondition) : IConditionRegistryMsg;
}
