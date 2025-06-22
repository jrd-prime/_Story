using System;
using System.Collections.Generic;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Systems.Use.Action.Strategies;
using _StoryGame.Infrastructure.Interact;

namespace _StoryGame.Game.Interact.Systems.Use.Action
{
    public sealed class UseActionStrategyProvider
    {
        private readonly Dictionary<EUseAction, IUseActionStrategy> _strategies = new();

        public UseActionStrategyProvider(InteractSystemDepFlyweight systemDep)

        {
            _strategies.Add(EUseAction.RoomExit, new ExitFromRoomStrategy(systemDep));
            _strategies.Add(EUseAction.PickUp, new PickUpStrategy(systemDep));
            _strategies.Add(EUseAction.Switch, new SwitchStrategy(systemDep));
        }

        public IUseActionStrategy GetStrategy(EUseAction action)
        {
            if (!_strategies.TryGetValue(action, out var strategy))
                throw new ArgumentException($"Unknown use action: {action}");

            return strategy;
        }
    }
}
