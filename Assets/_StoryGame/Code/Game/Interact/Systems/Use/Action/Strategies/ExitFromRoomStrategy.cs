using System;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Use.Action.Strategies
{
    public class ExitFromRoomStrategy : IUseActionStrategy
    {
        public string StrategyName => nameof(ExitFromRoomStrategy);

        private const int Price = 2;
        private IUsableExit _usableExit;

        private readonly InteractSystemDepFlyweight _dep;
        private readonly DialogResultHandler _dialogResultHandler;

        public ExitFromRoomStrategy(InteractSystemDepFlyweight dep)
        {
            _dep = dep;
            _dialogResultHandler = new DialogResultHandler(_dep.Log);

            _dialogResultHandler.AddCallback(EDialogResult.Apply, OnApplyAction);
            _dialogResultHandler.AddCallback(EDialogResult.Close, OnCloseAction);
        }

        private void OnCloseAction()
        {
        }

        public async UniTask<bool> ExecuteAsync(IUsable usable)
        {
            _usableExit = usable as IUsableExit ?? throw new Exception("Interact is not IUsableExit");

            var source = new UniTaskCompletionSource<EDialogResult>();

            var localizedName = _dep.LocalizationProvider.Localize(_usableExit.LocalizationKey, ETable.Words);
            var localizedQuestion =
                _dep.LocalizationProvider.Localize(_usableExit.ExitQuestionLocalizationKey, ETable.SmallPhrase);

            IUIViewerMsg msg = new ShowExitRoomWindowMsg(localizedName, localizedQuestion, Price, source);

            return await ProcessExitFromRoom(source, msg);
        }

        private async UniTask<bool> ProcessExitFromRoom(UniTaskCompletionSource<EDialogResult> source, IUIViewerMsg msg)
        {
            bool success;

            try
            {
                _dep.Publisher.ForUIViewer(msg);
                var result = await source.Task;

                success = _dialogResultHandler.HandleResult(result);
            }
            finally
            {
                source?.TrySetCanceled();
            }

            return success;
        }

        private void OnApplyAction()
        {
            _usableExit.SetState(EUseState.Used);
            _dep.Publisher.ForGameManager(new SpendEnergyMsg(Price));
            _dep.Publisher.ForGameManager(new ChooseNextRoomRequestMsg());
        }
    }
}
