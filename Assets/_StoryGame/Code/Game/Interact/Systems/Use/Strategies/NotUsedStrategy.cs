using System;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Data;
using _StoryGame.Game.Interact.Systems.Inspect;
using _StoryGame.Game.Interact.Systems.Use.Action;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Use.Strategies
{
    public class NotUsedStrategy : IUseSystemStrategy
    {
        public string StrategyName => nameof(NotUsedStrategy);

        private readonly InteractSystemDepFlyweight _dep;
        private readonly DialogResultHandler _dialogResultHandler;
        private IUsable _usable;
        private string _objLocalizedName;
        private readonly UseActionStrategyProvider _useActionStrategyProvider;

        public NotUsedStrategy(InteractSystemDepFlyweight dep)
        {
            _dep = dep;

            _useActionStrategyProvider = new UseActionStrategyProvider(dep);

            _dialogResultHandler = new DialogResultHandler();

            _dialogResultHandler.AddCallback(EDialogResult.Apply, OnApplyAction);
            _dialogResultHandler.AddCallback(EDialogResult.Close, OnCloseAction);
        }

        public async UniTask<bool> ExecuteAsync(IUsable interactable)
        {
            _dep.Publisher.ForUIViewer(new CurrentOperationMsg(StrategyName));

            _usable = interactable;
            _objLocalizedName =
                _dep.LocalizationProvider.Localize(_usable.LocalizationKey, ETable.Words,
                    ETextTransform.Upper);

            return await Use();
        }

        private async UniTask<bool> Use()
        {
            var strategy = _useActionStrategyProvider.GetStrategy(_usable.UseAction);
            return await strategy.ExecuteAsync(_usable);
        }


        private void OnApplyAction()
        {
            throw new NotImplementedException();
        }

        private void OnCloseAction()
        {
            throw new NotImplementedException();
        }
    }
}
