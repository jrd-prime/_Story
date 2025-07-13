using System.Collections.Generic;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Game.Interact.Interactables.Unlock;
using _StoryGame.Infrastructure.Interact;

namespace _StoryGame.Game.Interact.Systems.Toggle
{
    public sealed class ToggleStrategyProvider
    {
        private readonly Dictionary<EToggleType, IToggleSystemStrategy> _strategies = new();

        public ToggleStrategyProvider(InteractSystemDepFlyweight dep, ConditionChecker conditionChecker)
        {
            _strategies.Add(EToggleType.Responder, new ToggleResponderStrategy(dep, conditionChecker));
            _strategies.Add(EToggleType.Modifier, new ToggleModifierStrategy(dep, conditionChecker));
        }

        public IToggleSystemStrategy GetStrategy(EToggleType toggleType, ESwitchState switchState)
        {
            var strategy = _strategies[toggleType];
            strategy.SetState(switchState);
            return strategy;
        }
    }
}
