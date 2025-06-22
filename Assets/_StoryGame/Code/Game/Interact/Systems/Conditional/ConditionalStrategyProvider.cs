using System;
using System.Collections.Generic;
using _StoryGame.Core.Interactables;
using _StoryGame.Data.Interactable;
using _StoryGame.Infrastructure.Interactables;

namespace _StoryGame.Game.Interactables.Impls.Systems.Cond
{
    public sealed class ConditionalStrategyProvider
    {
        private readonly Dictionary<EConditionalState, IConditionalSystemStrategy> _strategies = new();

        public ConditionalStrategyProvider(InteracSystemDepFlyweight systemDep)
        {
            _strategies.Add(EConditionalState.Locked, new LockedObjectStrategy(systemDep));
            _strategies.Add(EConditionalState.Unlocked, new UnlockObjectStrategy(systemDep));
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
