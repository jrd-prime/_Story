using System;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data.Interact;
using _StoryGame.Game.Interact.SortMbDelete.InteractablesSORT;
using _StoryGame.Game.Interact.todecor.Abstract;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interact.todecor.Decorators.Active
{
    public sealed class APassableDecorator : ADecorator, IActiveDecorator
    {
        [Space(10)] [SerializeField] private PassableData passableData;
        private DialogResultHandler _dialogResultHandler;
        private IInteractable _interactable;

        public override int Priority => 10;

        protected override void InitializeInternal()
        {
            _dialogResultHandler = new DialogResultHandler(Dep.Log);

            _dialogResultHandler.AddCallback(EDialogResult.Apply, OnApplyAction);
            _dialogResultHandler.AddCallback(EDialogResult.Close, OnCloseAction);

            IsInitialized = true;
        }

        protected override async UniTask<EDecoratorResult> ProcessInternal(IInteractable interactable)
        {
            _interactable = interactable;
            var source = new UniTaskCompletionSource<EDialogResult>();

            var exitLocalizedName = Dep.L10n.Localize(interactable.LocalizationKey, ETable.Words);
            var localizedDoorAction = Dep.L10n.Localize(passableData.doorAction.ToString(), ETable.Words);

            var localizedRoomName = passableData.doorAction switch
            {
                EDoorAction.EnterQ => Dep.L10n.Localize(passableData.toRoom.ToString(), ETable.Words),
                EDoorAction.ExitQ => Dep.L10n.Localize(passableData.fromRoom.ToString(), ETable.Words),
                EDoorAction.AscendQ => Dep.L10n.Localize(passableData.toRoom.ToString(), ETable.Words),
                EDoorAction.DescendQ => Dep.L10n.Localize(passableData.toRoom.ToString(), ETable.Words),
                EDoorAction.NotSet => throw new ArgumentException($"{interactable.Name} DoorAction not set."),
                _ => throw new ArgumentOutOfRangeException()
            };

            var question = $"{localizedRoomName}: {localizedDoorAction}?";

            var msg = new ShowExitRoomWindowMsg(exitLocalizedName, question, passableData.usePrice, source);

            await ProcessExitFromRoom(source, msg);

            return EDecoratorResult.Success;
        }

        private async UniTask<bool> ProcessExitFromRoom(UniTaskCompletionSource<EDialogResult> source, IUIViewerMsg msg)
        {
            try
            {
                Dep.Publisher.ForUIViewer(msg);
                var result = await source.Task;

                await _dialogResultHandler.HandleResultAsync(result);
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
            Dep.Publisher.ForGameManager(new SpendEnergyRequestMsg(passableData.usePrice));
            Dep.Publisher.ForGameManager(new GoToRoomRequestMsg(passableData.exit, passableData.fromRoom,
                passableData.toRoom));

            return UniTask.CompletedTask;
        }
    }
}
