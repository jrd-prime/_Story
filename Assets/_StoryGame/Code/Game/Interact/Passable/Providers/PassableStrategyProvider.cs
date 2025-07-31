using System;
using System.Collections.Generic;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.Passable.Strategies;
using _StoryGame.Game.Managers.Condition;
using _StoryGame.Infrastructure.Interact;

namespace _StoryGame.Game.Interact.Passable.Providers
{
    public class PassableStrategyProvider
    {
        private readonly Dictionary<Type, Dictionary<Enum, IPassSystemStrategy>> _strategiesByType = new();

        public PassableStrategyProvider(InteractSystemDepFlyweight dep, ConditionChecker conditionChecker)
        {
            _strategiesByType.Add(typeof(Passable), new Dictionary<Enum, IPassSystemStrategy>()
            {
                { EPassableState.Locked, new LockedDoorStrategy(dep, conditionChecker) },
                { EPassableState.Unlocked, new UnlockedDoorStrategy(dep, conditionChecker) }
            });
        }

        public IPassSystemStrategy GetStrategy(IPassable interactable)
        {
            switch (interactable)
            {
                case Passable door:
                    return GetDoorStrategy(door);
                default:
                    throw new ArgumentOutOfRangeException(nameof(interactable), interactable, null);
            }
        }

        private IPassSystemStrategy GetDoorStrategy(Passable door)
        {
            if (!_strategiesByType.TryGetValue(door.GetType(), out var dictionary))
                throw new ArgumentException($"Unknown door type: {door.GetType()}");

            if (!dictionary.TryGetValue(door.PassableState, out var strategy))
                throw new ArgumentException($"Unknown door state: {door.PassableState}");

            return strategy;
        }
    }
}
