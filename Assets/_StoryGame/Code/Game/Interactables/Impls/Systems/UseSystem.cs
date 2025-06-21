using System;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data;
using _StoryGame.Game.Interactables.Abstract;
using _StoryGame.Game.Interactables.Impls.ObjTypes.Usable;
using _StoryGame.Game.Managers.Game.Messages;
using Cysharp.Threading.Tasks;
using VContainer;

namespace _StoryGame.Game.Interactables.Impls.Systems
{
    public sealed class UseSystem : AInteractableSystem<AUsable>
    {
        private AUsable _usable;

        public UseSystem(IObjectResolver resolver) : base(resolver)
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

            Log.Debug($"Interact <color=green>END</color>. Result: {result}");
            return result;
        }

        /// <summary>
        /// Первое взаимодействие
        /// </summary>
        /// <returns></returns>
        private async UniTask<bool> Use()
        {
            Log.Debug("<color=green>Use</color>".ToUpper());
            await OnStartUse();
            await OnEndUse();
            return true;
        }

        private async UniTask OnStartUse()
        {
            Log.Debug("On Start Use " + _usable.UsableAction);
            var action = _usable.UsableAction;

            switch (action)
            {
                case EUsableAction.NotSet: break;
                case EUsableAction.Switch:
                    Log.Debug("Switch");
                    break;
                case EUsableAction.PickUp:
                    Log.Debug("PickUp");
                    break;
                case EUsableAction.RoomExit:
                    Log.Debug("RoomExit");
                    await OnRoomExitAction();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await UniTask.Yield();
        }

        private async UniTask OnRoomExitAction()
        {
            Log.Debug("OnRoomExitAction");
            var exitObj = _usable as RoomDoor ?? throw new Exception("Interactable is not RoomDoor");

            var price = 2;
            var source = new UniTaskCompletionSource<EDialogResult>();

            var localizedQuestion =
                LocalizationProvider.Localize(exitObj.ExitQuestionLocalizationKey, ETable.SmallPhrase);

            IUIViewerMsg msg = new ShowExitRoomWindowMsg(LocalizedName, localizedQuestion, price, source);
            try
            {
                Publisher.ForUIViewer(msg);
                var result = await source.Task;

                if (result == EDialogResult.Apply)
                {
                    Publisher.ForGameManager(new SpendEnergyMsg(price));
                    Publisher.ForGameManager(new RoomChooseRequestMsg());
                }
                else if (result == EDialogResult.Close)
                {
                    Log.Debug("Leave room?   - CLOSE");
                }
                else Log.Warn("Unhandled result: " + result);
            }
            finally
            {
                source?.TrySetCanceled();
            }
        }

        private async UniTask<bool> Used()
        {
            Log.Debug("<color=green>Used</color>".ToUpper());
            await UniTask.Yield();
            return true;
        }

        private async UniTask<bool> OnEndUse()
        {
            Log.Debug("On Use Complete");
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
