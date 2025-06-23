using System;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Data;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Systems.Inspect;
using _StoryGame.Game.Interact.Systems.Use.Action;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Use.Strategies
{
    public class NotUsedStrategy : IUseSystemStrategy
    {
        public string StrategyName => nameof(NotUsedStrategy);

        private IUsable _usable;

        private readonly InteractSystemDepFlyweight _dep;
        private readonly UseActionStrategyProvider _useActionStrategyProvider;

        public NotUsedStrategy(InteractSystemDepFlyweight dep)
        {
            _dep = dep;

            _useActionStrategyProvider = new UseActionStrategyProvider(dep);
        }

        public async UniTask<bool> ExecuteAsync(IUsable interactable)
        {
            _dep.Publisher.ForUIViewer(new CurrentOperationMsg(StrategyName));

            _usable = interactable;

            var actionStrategy = _useActionStrategyProvider.GetStrategy(_usable.UseAction);
            return await actionStrategy.ExecuteAsync(_usable);
        }
    }
}
