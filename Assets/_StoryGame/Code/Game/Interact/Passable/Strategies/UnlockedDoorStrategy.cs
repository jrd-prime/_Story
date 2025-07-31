using System;
using System.Text;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Game.Interact.SortMbDelete.InteractablesSORT;
using _StoryGame.Game.Managers.Condition;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Passable.Strategies
{
    public sealed class UnlockedDoorStrategy : IPassSystemStrategy
    {
        public string Name => nameof(LockedDoorStrategy);

        private Passable _door;

        private readonly InteractSystemDepFlyweight _dep;
        private readonly DialogResultHandler _dialogResultHandler;
        private readonly ConditionChecker _conditionChecker;

        public UnlockedDoorStrategy(InteractSystemDepFlyweight dep, ConditionChecker conditionChecker)
        {
            _dep = dep;
            _conditionChecker = conditionChecker;
            _dialogResultHandler = new DialogResultHandler(_dep.Log);

            _dialogResultHandler.AddCallback(EDialogResult.Apply, OnApplyAction);
            _dialogResultHandler.AddCallback(EDialogResult.Close, OnCloseAction);
        }

        public async UniTask<bool> ExecuteAsync(IPassable interactable)
        {
            _door = interactable as Passable ?? throw new ArgumentException("Interactable is not a door");

            var source = new UniTaskCompletionSource<EDialogResult>();

            var exitLocalizedName = _dep.L10n.Localize(_door.LocalizationKey, ETable.Words);
            var localizedDoorAction = _dep.L10n.Localize(_door.PassableData.doorAction.ToString(), ETable.Words);

            var localizedRoomName = _door.PassableData.doorAction switch
            {
                EDoorAction.EnterQ => _dep.L10n.Localize(_door.PassableData.toRoom.ToString(), ETable.Words),
                EDoorAction.ExitQ => _dep.L10n.Localize(_door.PassableData.fromRoom.ToString(), ETable.Words),
                EDoorAction.AscendQ => _dep.L10n.Localize(_door.PassableData.toRoom.ToString(), ETable.Words),
                EDoorAction.DescendQ => _dep.L10n.Localize(_door.PassableData.toRoom.ToString(), ETable.Words),
                EDoorAction.NotSet => throw new ArgumentException($"{_door.Name} DoorAction not set."),
                _ => throw new ArgumentOutOfRangeException()
            };

            var question = GetQuestion(localizedRoomName, localizedDoorAction);

            IUIViewerMsg msg = new ShowExitRoomWindowMsg(exitLocalizedName, question, _door.PassableData.usePrice, source);

            return await ProcessExitFromRoom(source, msg);
        }

        private string GetQuestion(string localizedRoomName, string localizedDoorAction)
        {
            var itemConditions = _conditionChecker.GetItemConditionThought(_door.ConditionsData.oneOfItem);
            var questionBuilder = new StringBuilder();

            if (itemConditions.Item1)
                questionBuilder.AppendLine(_dep.L10n.Localize(itemConditions.Item2, ETable.SmallPhrase));

            questionBuilder.AppendLine($"{localizedRoomName}: {localizedDoorAction}?");

            return questionBuilder.ToString();
        }

        private async UniTask<bool> ProcessExitFromRoom(UniTaskCompletionSource<EDialogResult> source, IUIViewerMsg msg)
        {
            try
            {
                _dep.Publisher.ForUIViewer(msg);
                var result = await source.Task;

                _dialogResultHandler.HandleResultAsync(result);
            }
            catch (Exception e)
            {
                throw new Exception("Error in ProcessExitFromRoom", e);
            }
            finally
            {
                source?.TrySetCanceled();
            }

            return true;
        }

        private UniTask OnCloseAction() => UniTask.CompletedTask; 

        private UniTask OnApplyAction()
        {
            // _usableExit.SetState(EUseState.Used); // TODO сбрасывать на активации комнат
            _dep.Publisher.ForGameManager(new SpendEnergyRequestMsg(_door.PassableData.usePrice));
            _dep.Publisher.ForGameManager(new GoToRoomRequestMsg(_door.PassableData.exit, _door.PassableData.fromRoom,
                _door.PassableData.toRoom));

            return UniTask.CompletedTask;
        }
    }
}
