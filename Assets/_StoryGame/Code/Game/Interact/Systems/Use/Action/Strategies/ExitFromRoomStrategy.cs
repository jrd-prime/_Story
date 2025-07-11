using System;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Game.Interact.Interactables.Use;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Use.Action.Strategies
{
    public class ExitFromRoomStrategy : IUseActionStrategy
    {
        public string Name => nameof(ExitFromRoomStrategy);

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

            var exitLocalizedName = _dep.L10n.Localize(_usableExit.LocalizationKey, ETable.Words);
            var localizedQuestion =
                _dep.L10n.Localize(_usableExit.DoorAction.ToString(), ETable.Words);

            var locTransKey = _usableExit.DoorAction switch
            {
                EDoorAction.EnterQ => _dep.L10n.Localize(_usableExit.TransitionToRoom.ToString(),
                    ETable.Words),
                EDoorAction.ExitQ => _dep.L10n.Localize(_usableExit.RoomType.ToString(), ETable.Words),
                EDoorAction.NotSet => throw new ArgumentException($"{_usableExit.Name} DoorAction not set."),
                _ => throw new ArgumentOutOfRangeException()
            };

            var lo = $"{locTransKey}: {localizedQuestion}?";

            IUIViewerMsg msg = new ShowExitRoomWindowMsg(exitLocalizedName, lo, Price, source);

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
            // _usableExit.SetState(EUseState.Used); // TODO сбрасывать на активации комнат
            _dep.Publisher.ForGameManager(new SpendEnergyMsg(Price));
            // _dep.Publisher.ForGameManager(new GoToRoomRequestMsg(_usableExit.TransitionToRoom));
        }
    }
}
