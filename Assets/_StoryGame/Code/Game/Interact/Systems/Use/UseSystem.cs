using System;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.ObjTypes.Usable;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems
{
    public sealed class UseSystem : AInteractSystem<AUsable>
    {
        private AUsable _usable;

        public UseSystem(InteractSystemDepFlyweight systemDep) : base(systemDep)
        {
        }

        protected override void OnPreProcess(AUsable interactable)
        {
            _usable = interactable;
        }

        protected override async UniTask<bool> OnProcessAsync()
        {
            var result = _usable.UseState switch
            {
                EUseState.NotUsed => await Use(),
                EUseState.Used => await Used(),
                _ => false
            };

            SystemDep.Log.Debug($"Interact <color=green>END</color>. Result: {result}");
            return result;
        }

        /// <summary>
        /// Первое взаимодействие
        /// </summary>
        /// <returns></returns>
        private async UniTask<bool> Use()
        {
            SystemDep.Log.Debug("<color=green>Use</color>".ToUpper());
            await OnStartUse();
            await OnEndUse();
            return true;
        }

        private async UniTask OnStartUse()
        {
            SystemDep.Log.Debug("On Start Use " + _usable.UsableAction);
            var action = _usable.UsableAction;

            switch (action)
            {
                case EUsableAction.NotSet: break;
                case EUsableAction.Switch:
                    SystemDep.Log.Debug("Switch");
                    break;
                case EUsableAction.PickUp:
                    SystemDep.Log.Debug("PickUp");
                    break;
                case EUsableAction.RoomExit:
                    SystemDep.Log.Debug("RoomExit");
                    await OnRoomExitAction();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await UniTask.Yield();
        }

        private async UniTask OnRoomExitAction()
        {
            SystemDep.Log.Debug("OnRoomExitAction");
            var exitObj = _usable as RoomDoor ?? throw new Exception("Interact is not RoomDoor");

            var price = 2;
            var source = new UniTaskCompletionSource<EDialogResult>();

            var localizedQuestion =
                SystemDep.LocalizationProvider.Localize(exitObj.ExitQuestionLocalizationKey, ETable.SmallPhrase);

            IUIViewerMsg msg = new ShowExitRoomWindowMsg(LocalizedName, localizedQuestion, price, source);
            try
            {
                SystemDep.Publisher.ForUIViewer(msg);
                var result = await source.Task;

                if (result == EDialogResult.Apply)
                {
                    SystemDep.Publisher.ForGameManager(new SpendEnergyMsg(price));
                    SystemDep.Publisher.ForGameManager(new RoomChooseRequestMsg());
                }
                else if (result == EDialogResult.Close)
                {
                    SystemDep.Log.Debug("Leave room?   - CLOSE");
                }
                else SystemDep.Log.Warn("Unhandled result: " + result);
            }
            finally
            {
                source?.TrySetCanceled();
            }
        }

        private async UniTask<bool> Used()
        {
            SystemDep.Log.Debug("<color=green>Used</color>".ToUpper());
            await UniTask.Yield();
            return true;
        }

        private async UniTask<bool> OnEndUse()
        {
            SystemDep.Log.Debug("On Use Complete");
            await UniTask.Yield();
            return true;
        }
    }

    public record RoomChooseRequestMsg : IGameManagerMsg
    {
    }

    public record ShowExitRoomWindowMsg(
        string LocalizedName,
        string Question,
        int Price,
        UniTaskCompletionSource<EDialogResult> CompletionSource) : IUIViewerMsg;
}
