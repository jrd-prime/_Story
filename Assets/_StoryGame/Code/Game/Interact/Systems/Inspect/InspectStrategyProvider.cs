using System;
using System.Collections.Generic;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Game.Interact.Systems.Inspect.Strategies;
using _StoryGame.Infrastructure.Interact;

namespace _StoryGame.Game.Interact.Systems.Inspect
{
    /// <summary>
    /// Provides strategies for handling different inspectable states of interactable objects.
    /// </summary>
    public sealed class InspectStrategyProvider
    {
        private readonly Dictionary<EInspectState, IInspectSystemStrategy> _strategies = new();

        public InspectStrategyProvider(InteractSystemDepFlyweight systemDep)
        {
            _strategies.Add(EInspectState.NotInspected, new NotInspectedStrategy(systemDep));
            _strategies.Add(EInspectState.Inspected, new AlreadyInspectedStrategy(systemDep));
            _strategies.Add(EInspectState.Searched, new SearchStrategy(systemDep));
        }

        /// <summary>
        /// Gets the strategy implementation for the specified inspect state.
        /// </summary>
        public IInspectSystemStrategy GetStrategy(EInspectState state)
        {
            if (!_strategies.TryGetValue(state, out var strategy))
                throw new ArgumentException($"Unknown conditional state: {state}");

            return strategy;
        }
    }
}
