using System;
using System.Collections.Generic;
using _StoryGame.Core.Interact;
using _StoryGame.Data.Interact;
using _StoryGame.Game.Interact.Systems.Conditional.Strategies;
using _StoryGame.Infrastructure.Interact;

namespace _StoryGame.Game.Interact.Systems.Conditional
{
    public sealed class ConditionalStrategyProvider
    {
        private readonly Dictionary<EConditionalState, IConditionalSystemStrategy> _strategies = new();

        public ConditionalStrategyProvider(InteractSystemDepFlyweight systemDep)
        {
            _strategies.Add(EConditionalState.Locked, new LockedStrategy(systemDep));
            _strategies.Add(EConditionalState.Unlocked, new UnlockStrategy(systemDep));
            _strategies.Add(EConditionalState.Looted, new AlreadyLootedStrategy(systemDep));
        }

        public IConditionalSystemStrategy GetStrategy(EConditionalState state)
        {
            if (!_strategies.TryGetValue(state, out var strategy))
                throw new ArgumentException($"Unknown conditional state: {state}");

            return strategy;
        }
    }
}
