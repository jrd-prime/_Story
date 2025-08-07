using System;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data.Interact;
using _StoryGame.Game.Interact.SortMbDelete.InteractablesSORT;
using _StoryGame.Game.Interact.todecor.Abstract;
using _StoryGame.Game.Interact.todecor.Impl.DeviceSystems;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace _StoryGame.Game.Interact.todecor.Decorators.Active
{
    public sealed class ADeviceDecorator : ADecorator, IActiveDecorator
    {
        [Space(10)] [SerializeField] private ADeviceUI deviceUI;
        [SerializeField] private DeviceData deviceData;
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

        private void Awake()
        {
            if (!deviceUI)
            {
                Dep.Log.Error("DeviceUI not found on " + name);
                enabled = false;
                return;
            }

            if (!deviceData)
            {
                Dep.Log.Error("DeviceData not found on " + name);
                enabled = false;
                return;
            }
        }

        protected override async UniTask<EDecoratorResult> ProcessInternal(IInteractable interactable)
        {
            _interactable = interactable;
            
            
            // показать панель
            deviceUI.ShowPanel();
            
            // ожидание результат  - закрыть панель, подключить флеш с кодом или открыть дверь
            // 
            
            
            
            var source = new UniTaskCompletionSource<EDialogResult>();

            var exitLocalizedName = Dep.L10n.Localize(interactable.LocalizationKey, ETable.Words);
            var localizedDoorAction = Dep.L10n.Localize(deviceData.doorAction.ToString(), ETable.Words);

       

            var question = $": {localizedDoorAction}?";

            var msg = new ShowExitRoomWindowMsg(exitLocalizedName, question, deviceData.usePrice, source);

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
            Dep.Publisher.ForGameManager(new SpendEnergyRequestMsg(deviceData.usePrice));
            Dep.Publisher.ForGameManager(new GoToRoomRequestMsg(deviceData.exit, deviceData.fromRoom,
                deviceData.toRoom));

            return UniTask.CompletedTask;
        }
    }
}
