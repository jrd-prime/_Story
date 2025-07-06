using System;
using System.Collections.Generic;
using _StoryGame.Core.Interact;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Systems.Use.Strategies;
using _StoryGame.Infrastructure.Interact;

namespace _StoryGame.Game.Interact.Systems.Use
{
    public sealed class UseStrategyProvider
    {
        private readonly Dictionary<EUseState, IUseSystemStrategy> _strategies = new();

        public UseStrategyProvider(InteractSystemDepFlyweight systemDep)
        {
            _strategies.Add(EUseState.NotUsed, new NotUsedStrategy(systemDep));
            _strategies.Add(EUseState.Used, new AlreadyUsedStrategy(systemDep));
        }

        /// <summary>
        /// Gets the strategy implementation for the specified use state.
        /// </summary>
        public IUseSystemStrategy GetStrategy(EUseState state)
        {
            if (!_strategies.TryGetValue(state, out var strategy))
                throw new ArgumentException($"Unknown conditional state: {state}");

            return strategy;
        }
    }
}
