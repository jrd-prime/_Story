using System;
using System.Collections.Generic;
using _StoryGame.Core.Interact;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Interactables;
using _StoryGame.Game.Interact.Interactables.Unlock;
using _StoryGame.Game.Interact.Systems.Inspect;
using _StoryGame.Game.Interact.Systems.Unlockable.Strategies;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems
{
    public sealed class UnlockSystem : AInteractSystem<IUnlockable>
    {
        private readonly UnlockableStrategyProvider _strategyProvider;

        public UnlockSystem(InteractSystemDepFlyweight dep, UnlockableStrategyProvider strategyProvider) : base(dep) =>
            _strategyProvider = strategyProvider;

        protected override UniTask<bool> OnInteractAsync()
        {
            var strategy = _strategyProvider.GetStrategy(Interactable);
            Dep.Publisher.ForUIViewer(new CurrentOperationMsg(strategy.Name));
            return strategy.ExecuteAsync(Interactable);
        }
    }

    public class UnlockableStrategyProvider
    {
        private readonly Dictionary<Type, Dictionary<Enum, IUnlockSystemStrategy>> _strategiesByType = new();

        public UnlockableStrategyProvider(InteractSystemDepFlyweight dep, ConditionChecker conditionChecker)
        {
            _strategiesByType.Add(typeof(UnlockableDoor), new Dictionary<Enum, IUnlockSystemStrategy>()
            {
                { EDoorState.Locked, new LockedDoorStrategy(dep, conditionChecker) },
                { EDoorState.Unlocked, new UnlockedDoorStrategy(dep) }
            });
        }

        public IUnlockSystemStrategy GetStrategy(IUnlockable interactable)
        {
            switch (interactable)
            {
                case UnlockableDoor door:
                    return GetDoorStrategy(door);
                default:
                    throw new ArgumentOutOfRangeException(nameof(interactable), interactable, null);
            }
        }

        private IUnlockSystemStrategy GetDoorStrategy(UnlockableDoor door)
        {
            if (!_strategiesByType.TryGetValue(door.GetType(), out var dictionary))
                throw new ArgumentException($"Unknown door type: {door.GetType()}");

            if (!dictionary.TryGetValue(door.DoorState, out var strategy))
                throw new ArgumentException($"Unknown door state: {door.DoorState}");

            return strategy;
        }
    }
}
