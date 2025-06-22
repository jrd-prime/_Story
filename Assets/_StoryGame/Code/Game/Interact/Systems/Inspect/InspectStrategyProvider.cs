using System;
using System.Collections.Generic;
using _StoryGame.Core.Interact;
using _StoryGame.Data.Interact;
using _StoryGame.Game.Interact.Systems.Inspect;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Conditional
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
            _strategies.Add(EInspectState.Inspected, new InspectedStrategy(systemDep));
            _strategies.Add(EInspectState.Searched, new SearchedStrategy(systemDep));
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


    // Стратегия для NotInspected
    public sealed class NotInspectedStrategy : IInspectSystemStrategy
    {
        public string StrategyName => nameof(NotInspectedStrategy);

        private readonly InteractSystemDepFlyweight _systemDep;

        public NotInspectedStrategy(InteractSystemDepFlyweight systemDep) => _systemDep = systemDep;

        public async UniTask<bool> ExecuteAsync(IInspectable inspectable)
        {
            _systemDep.Publisher.ForUIViewer(new CurrentOperationMsg("Inspect"));
            await InspectAsync(inspectable);
            return true;
        }

        private async UniTask InspectAsync(IInspectable inspectable)
        {
            // Реализация инспекции аналогичная оригинальному коду
        }
    }

// Стратегия для Inspected
    public sealed class InspectedStrategy : IInspectSystemStrategy
    {
        public string StrategyName => nameof(InspectedStrategy);

        private readonly InteractSystemDepFlyweight _systemDep;

        public InspectedStrategy(InteractSystemDepFlyweight systemDep) => _systemDep = systemDep;

        public async UniTask<bool> ExecuteAsync(IInspectable inspectable)
        {
            await ShowLootTipAfterInspect(inspectable);
            return true;
        }

        private async UniTask ShowLootTipAfterInspect(IInspectable inspectable)
        {
            // Реализация показа подсказки после инспекции
        }
    }

// Стратегия для Searched
    public sealed class SearchedStrategy : IInspectSystemStrategy
    {
        public string StrategyName => nameof(SearchedStrategy);

        private readonly InteractSystemDepFlyweight _systemDep;

        public SearchedStrategy(InteractSystemDepFlyweight systemDep) => _systemDep = systemDep;

        public async UniTask<bool> ExecuteAsync(IInspectable inspectable)
        {
            await ShowLootTipAfterSearch(inspectable);
            return true;
        }

        private async UniTask ShowLootTipAfterSearch(IInspectable inspectable)
        {
            // Реализация показа подсказки после поиска
        }
    }
}
