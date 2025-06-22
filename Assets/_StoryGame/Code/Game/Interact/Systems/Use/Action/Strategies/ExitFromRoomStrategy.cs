using System;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Use.Action.Strategies
{
    public class ExitFromRoomStrategy : IUseActionStrategy
    {
        private readonly InteractSystemDepFlyweight _dep;
        private IUsableExit _usableExit;
        private readonly DialogResultHandler _dialogResultHandler;
        public string StrategyName => nameof(ExitFromRoomStrategy);
        private const int Price = 2;
        
        public ExitFromRoomStrategy(InteractSystemDepFlyweight dep)
        {
            _dep = dep;
            _dialogResultHandler = new DialogResultHandler();

            _dialogResultHandler.AddCallback(EDialogResult.Apply, OnApplyAction);
            _dialogResultHandler.AddCallback(EDialogResult.Close, OnCloseAction);
        }

        private void OnCloseAction()
        {
        }

        private void OnApplyAction()
        {
            _dep.Publisher.ForGameManager(new SpendEnergyMsg(Price));
            _dep.Publisher.ForGameManager(new RoomChooseRequestMsg());
        }

        public async UniTask<bool> ExecuteAsync(IUsable usable)
        {
            _usableExit = usable as IUsableExit ?? throw new Exception("Interact is not IUsableExit");
            return await ExitFromRoom();
        }

        private async UniTask<bool> ExitFromRoom()
        {
            var source = new UniTaskCompletionSource<EDialogResult>();

            var localizedQuestion =
                _dep.LocalizationProvider.Localize(_usableExit.ExitQuestionLocalizationKey, ETable.SmallPhrase);

            IUIViewerMsg msg = new ShowExitRoomWindowMsg("LocalizedName", localizedQuestion, Price, source);
            try
            {
                _dep.Publisher.ForUIViewer(msg);
                var result = await source.Task;

                _dialogResultHandler.HandleResult(result);
            }
            finally
            {
                source?.TrySetCanceled();
            }

            return true;
        }
    }
}
