using System;
using System.Collections.Generic;
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
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace _StoryGame.Game.Interact.todecor.Decorators.Active
{
    public sealed class ADeviceDecorator : ADecorator, IActiveDecorator
    {
        [Space(10)] [SerializeField] private ADeviceUI deviceUI;
        [SerializeField] private EDeviceSystem deviceSystem = EDeviceSystem.NotSet;
        [SerializeField] private DeviceData deviceData;

        public override int Priority => 10;

        private DialogResultHandler _dialogResultHandler;
        private IInteractable _interactable;

        private readonly Dictionary<EDeviceSystem, object> _deviceSystems = new()
        {
            { EDeviceSystem.B1AccessControlPanel, new AccessControlPanelSystem() },
        };

        protected override void InitializeInternal()
        {
            if (!deviceUI)
            {
                Dep.Log.Error("DeviceUI not found on " + name);
                enabled = false;
                return;
            }

            if (deviceSystem == EDeviceSystem.NotSet)
            {
                Dep.Log.Error("DeviceSystem not set on " + name);
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

        private ADeviceSystem<T> GetSystem<T>(EDeviceSystem eDeviceSystem) where T : Enum
        {
            if (_deviceSystems.TryGetValue(eDeviceSystem, out var system))
                return (ADeviceSystem<T>)system;

            Dep.Log.Error("DeviceSystem not found in cache on " + name);
            enabled = false;
            return null;
        }

        protected override async UniTask<EDecoratorResult> ProcessInternal(IInteractable interactable)
        {
            if (IsInitialized == false)
            {
                Dep.Log.Error($"{this} Not Initialized.");
                enabled = false;
                return EDecoratorResult.Error;
            }

            _interactable = interactable;

            var result = EDecoratorResult.Error;
            switch (deviceSystem)
            {
                case EDeviceSystem.B1AccessControlPanel:
                    result = await RunAccessControlPanelSystem();
                    break;
                case EDeviceSystem.B2ServerControlPanel:
                    result = await RunServerControlPanelSystem();
                    break;
                case EDeviceSystem.NotSet:
                default:
                    throw new ArgumentOutOfRangeException();
            }


            return result;

            var source = new UniTaskCompletionSource<EDialogResult>();

            var exitLocalizedName = Dep.L10n.Localize(interactable.LocalizationKey, ETable.Words);
            var localizedDoorAction = Dep.L10n.Localize(deviceData.doorAction.ToString(), ETable.Words);


            var question = $": {localizedDoorAction}?";

            var msg = new ShowExitRoomWindowMsg(exitLocalizedName, question, deviceData.usePrice, source);


            return EDecoratorResult.Success;
        }

        private async UniTask<EDecoratorResult> RunServerControlPanelSystem()
        {
            return EDecoratorResult.Success;
        }

        private async UniTask<EDecoratorResult> RunAccessControlPanelSystem()
        {
            var devSavedData = new DeviceSavedData<EAccessControlPanelState>()
                { state = EAccessControlPanelState.NoCode };

            var system = GetSystem<EAccessControlPanelState>(deviceSystem);

            var result = await system.Initialize(deviceUI, devSavedData, _interactable, Dep);

            if (result == EDecoratorResult.Error)
                return result;

            result = await system.Process();

            return result;
        }

        private UniTask OnApplyAction()
        {
            // _usableExit.SetState(EUseState.Used); // TODO сбрасывать на активации комнат
            Dep.Publisher.ForGameManager(new SpendEnergyRequestMsg(deviceData.usePrice));
            Dep.Publisher.ForGameManager(new GoToRoomRequestMsg(deviceData.exit, deviceData.fromRoom,
                deviceData.toRoom));

            return UniTask.CompletedTask;
        }
    }

    public enum EServerControlPanelState
    {
        NotSet = -1,
        NotConnected = 0,
        Connected = 1
    }

    public abstract class ADeviceSystem<TDeviceState> where TDeviceState : Enum
    {
        private TDeviceState _state;

        /// <summary>
        /// Тут передадим зависимости, а также сохраненные данные
        /// </summary>
        public abstract UniTask<EDecoratorResult> Initialize(ADeviceUI deviceUI,
            DeviceSavedData<TDeviceState> deviceData,
            IInteractable interactable, InteractSystemDepFlyweight dep);

        public abstract UniTask<EDecoratorResult> Process();
    }

    [Serializable]
    public struct DeviceSavedData<TDeviceState> where TDeviceState : Enum
    {
        public TDeviceState state;
    }

    internal enum EDeviceSystem
    {
        NotSet = -1,
        B1AccessControlPanel = 0,
        B2ServerControlPanel = 1
    }

    public enum EAccessControlPanelState
    {
        NotSet = -1,
        NoCode = 0,
        HasCode = 1
    }

    public sealed class AccessControlPanelSystem : ADeviceSystem<EAccessControlPanelState>
    {
        private AccessControlPanelDeviceUI _deviceUI;
        private DialogResultHandler<EAccessControlPanelDialogResult> _dialogResultHandler;
        private InteractSystemDepFlyweight _dep;

        public override UniTask<EDecoratorResult> Initialize(
            ADeviceUI deviceUI,
            DeviceSavedData<EAccessControlPanelState> deviceData,
            IInteractable interactable,
            InteractSystemDepFlyweight dep
        )
        {
            _dep = dep;
            _dialogResultHandler = new DialogResultHandler<EAccessControlPanelDialogResult>(_dep.Log);

            _dialogResultHandler.AddCallback(EAccessControlPanelDialogResult.FlashNotFound, OnFlashNotFoundAction);
            _dialogResultHandler.AddCallback(EAccessControlPanelDialogResult.InsertFlash, OnInsertFlashAction);
            _dialogResultHandler.AddCallback(EAccessControlPanelDialogResult.Close, OnCloseAction);
            _dialogResultHandler.AddCallback(EAccessControlPanelDialogResult.OpenGate, OnOpenGateAction);

            _deviceUI = deviceUI as AccessControlPanelDeviceUI ??
                        throw new ArgumentException("Not AccessControlPanelDeviceUI");

            return UniTask.FromResult(EDecoratorResult.Success);
        }

        public override async UniTask<EDecoratorResult> Process()
        {
            // показать панель


            var source = new UniTaskCompletionSource<EAccessControlPanelDialogResult>();
            var da = new DevDa<EAccessControlPanelDialogResult>(source);
            try
            {
                _deviceUI.ShowPanel(da);
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

            return EDecoratorResult.Success;
        }

        private UniTask OnOpenGateAction()
        {
            _dep.Log.Debug("OnOpenGateAction");
            return UniTask.CompletedTask;
        }

        private UniTask OnCloseAction()
        {
            _dep.Log.Debug("OnCloseAction");
            return UniTask.CompletedTask;
        }

        private UniTask OnFlashNotFoundAction()
        {
            _dep.Log.Debug("OnFlashNotFoundAction");
            return UniTask.CompletedTask;
        }

        private UniTask OnInsertFlashAction()
        {
            _dep.Log.Debug("OnInsertFlashAction");
            return UniTask.CompletedTask;
        }
    }

    public enum EAccessControlPanelDialogResult
    {
        InsertFlash = 0,
        FlashNotFound = 1,
        Close = 2,
        OpenGate = 3
    }
}
