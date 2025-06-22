using System;
using _StoryGame.Core.Animations.Messages;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Data;
using _StoryGame.Data.Animator;
using _StoryGame.Data.Interactable;
using _StoryGame.Data.Loot;
using _StoryGame.Game.Interactables.Abstract;
using _StoryGame.Game.Interactables.Interfaces;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.Interactables;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interactables.Impls.Systems
{
    //TODO NEED REFACTOR (state machine etc)
    public sealed class InspectSystem : AInteractSystem<IInspectable>
    {
        private const float InspectDuration = 2f;
        private const float SearchDuration = 2f;

        private EInspectState _inspectState;
        private IInspectable _inspectable;

        public InspectSystem(InteracSystemDepFlyweight systemDep) : base(systemDep)
        {
        }

        protected override void OnPreProcess(IInspectable interactable)
        {
            _inspectable = interactable;
            _inspectState = _inspectable.InspectState;
        }

        protected override async UniTask<bool> OnProcessAsync()
        {
            var result = _inspectState switch
            {
                EInspectState.NotInspected => await StartInspect(),
                EInspectState.Inspected => await Inspected(),
                EInspectState.Searched => await Searched(),
                _ => false
            };

            await OnInteractionComplete();

            SystemDep.Log.Debug($"Interact <color=green>END</color>. Result: {result}");
            return result;
        }

        private async UniTask<bool> StartInspect()
        {
            SystemDep.Publisher.ForUIViewer(new CurrentOperationMsg("Inspect"));
            await OnStartInspect();
            await Inspected();
            return true;
        }

        private async UniTask<bool> Inspected()
        {
            SystemDep.Log.Debug("<color=green>Inspected</color>".ToUpper());
            await OnCompleteInspect();
            return true;
        }

        private async UniTask<bool> StartSearch()
        {
            SystemDep.Publisher.ForUIViewer(new CurrentOperationMsg("Search"));
            await OnStartSearch();
            await Searched();
            return true;
        }

        private async UniTask<bool> Searched()
        {
            await OnCompleteSearch();
            return true;
        }

        private async UniTask OnInteractionComplete()
        {
            SystemDep.Log.Debug("--- OnInteractionComplete");
            await UniTask.CompletedTask;
        }

        private async UniTask OnStartInspect()
        {
            var source = new UniTaskCompletionSource<EDialogResult>();
            var message = new DisplayProgressBarMsg("Inspect", InspectDuration, source);

            try
            {
                SystemDep.Publisher.ForPlayerAnimator(new SetBoolMsg(AnimatorConst.IsGatherHigh, true));

                SystemDep.Publisher.ForPlayerOverHeadUI(message);
                var result = await source.Task;

                SystemDep.Publisher.ForPlayerAnimator(new SetBoolMsg(AnimatorConst.IsGatherHigh, false));

                if (result == EDialogResult.Close)
                {
                    SystemDep.Log.Debug("Inspect progress complete - CLOSE");
                }
                else SystemDep.Log.Warn("Unhandled result: " + result);
            }
            finally
            {
                source?.TrySetCanceled();
            }
        }

        private async UniTask OnCompleteInspect()
        {
            _inspectable.SetInspectState(EInspectState.Inspected);
            await ShowLootTipAfterInspect();
        }

        private async UniTask ShowLootTipAfterInspect()
        {
            var lootData = Room.GetLoot(InteractableId);

            if (lootData == null)
                throw new Exception("ShowLootTipAfterInspect - no loot data");

            var hasLoot = Room.HasLoot(InteractableId);

            var tipLocalizationId = hasLoot
                ? SystemDep.InteractableSystemTipData.GetRandomTip(EInteractableSystemTip.InspHasLoot)
                : SystemDep.InteractableSystemTipData.GetRandomTip(EInteractableSystemTip.InspNoLoot);

            var tip = SystemDep.LocalizationProvider.Localize(tipLocalizationId, ETable.SmallPhrase);

            var source = new UniTaskCompletionSource<EDialogResult>();

            IUIViewerMsg msg = hasLoot
                ? new ShowHasLootWindowMsg(LocalizedName, tip, lootData, source)
                : new ShowNoLootWindowMsg(LocalizedName.ToUpper(), tip, source);

            try
            {
                SystemDep.Log.Debug(hasLoot
                    ? "<color=green>Inspect - has loot</color>"
                    : "<color=red>Inspect - no loot</color>");

                SystemDep.Publisher.ForUIViewer(msg);
                var result = await source.Task;

                if (result == EDialogResult.Search)
                {
                    SystemDep.Publisher.ForGameManager(new SpendEnergyMsg(1));
                    await StartSearch();
                }
                else if (result == EDialogResult.Close)
                {
                    SystemDep.Log.Debug("ShowLootTipAfterInspect - CLOSE");
                }
                else
                {
                    SystemDep.Log.Warn("Unhandled result: " + result);
                }
            }
            finally
            {
                source?.TrySetCanceled();
            }
        }

        private async UniTask OnStartSearch()
        {
            var source = new UniTaskCompletionSource<EDialogResult>();

            SystemDep.Publisher.ForPlayerAnimator(new SetBoolMsg(AnimatorConst.IsGatherHigh, true));

            SystemDep.Publisher.ForPlayerOverHeadUI(new DisplayProgressBarMsg("Search", SearchDuration, source));
            await source.Task;

            SystemDep.Publisher.ForPlayerAnimator(new SetBoolMsg(AnimatorConst.IsGatherHigh, false));
        }


        private async UniTask OnCompleteSearch()
        {
            _inspectable.SetInspectState(EInspectState.Searched);
            await ShowLootTipAfterSearch();
        }

        private async UniTask ShowLootTipAfterSearch()
        {
            SystemDep.Publisher.ForUIViewer(new CurrentOperationMsg("ShowLootTipAfterSearch"));

            var source = new UniTaskCompletionSource<EDialogResult>();
            var lootData = Room.GetLoot(InteractableId);
            var message = new ShowLootWindowMsg(LocalizedName, lootData, source);

            try
            {
                SystemDep.Publisher.ForUIViewer(message);

                var result = await source.Task;
                source = null;

                if (result == EDialogResult.TakeAll)
                {
                    SystemDep.Log.Debug("ShowLootTipAfterSearch - TAKE ALL");
                    await OnTakeAllLoot(lootData);
                }
                else if (result == EDialogResult.Close)
                {
                    SystemDep.Log.Debug("ShowLootTipAfterSearch - CLOSE");
                }
                else SystemDep.Log.Warn("Unhandled result: " + result);
            }
            finally
            {
                source?.TrySetCanceled();
            }
        }

        private UniTask OnTakeAllLoot(InspectableData lootData)
        {
            SystemDep.Publisher.ForUIViewer(new CurrentOperationMsg("ShowLootTipAfterSearch"));

            _inspectable.CanInteract = false;
            SystemDep.Publisher.ForGameManager(new TakeRoomLootMsg(lootData));

            return UniTask.CompletedTask;
        }
    }

    public record CurrentOperationMsg(string CurrentOperation) : IUIViewerMsg;
}
