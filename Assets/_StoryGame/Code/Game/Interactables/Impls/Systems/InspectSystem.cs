using System;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Data.Interactable;
using _StoryGame.Data.Loot;
using _StoryGame.Game.Interactables.Abstract;
using _StoryGame.Game.Interactables.Data;
using _StoryGame.Game.Interactables.Impls.Inspect;
using _StoryGame.Game.Interactables.Interfaces;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using Cysharp.Threading.Tasks;
using VContainer;

namespace _StoryGame.Game.Interactables.Impls.Systems
{
    //TODO NEED REFACTOR (state machine etc)
    public sealed class InspectSystem : AInteractableSystem<IInspectable>
    {
        private const float InspectDuration = 2f;
        private const float SearchDuration = 2f;

        private readonly InspectSystemTipData _inspectSystemTipData;

        public InspectSystem(IObjectResolver resolver) : base(resolver)
        {
            _inspectSystemTipData = SettingsProvider.GetSettings<InspectSystemTipData>();
        }

        protected override async UniTask<bool> OnProcess()
        {
            var inspectState = Interactable.InspectState;
            var result = inspectState switch
            {
                EInspectState.NotInspected => await StartInspect(),
                EInspectState.Inspected => await Inspected(),
                EInspectState.Searched => await Searched(),
                _ => false
            };

            await OnInteractionComplete();

            Log.Debug($"Interact <color=green>END</color>. Result: {result}");
            return result;
        }

        private async UniTask<bool> StartInspect()
        {
            Log.Debug("<color=green>Start Inspect</color>".ToUpper());
            await OnStartInspect();
            await Inspected();
            return true;
        }

        private async UniTask<bool> Inspected()
        {
            Log.Debug("<color=green>Inspected</color>".ToUpper());
            await OnCompleteInspect();
            return true;
        }

        private async UniTask<bool> StartSearch()
        {
            Log.Debug("<color=green>Start Search</color>".ToUpper());
            await OnStartSearch();
            await Searched();
            return true;
        }

        private async UniTask<bool> Searched()
        {
            Log.Debug("<color=green>Searched</color>".ToUpper());
            await OnCompleteSearch();
            return true;
        }

        private async UniTask OnInteractionComplete()
        {
            Log.Debug("--- OnInteractionComplete");
            await UniTask.Yield();
        }

        private async UniTask OnStartInspect()
        {
            // anim hero // await progress bar
            Log.Debug("OnStart Inspect");

            var source = new UniTaskCompletionSource<EDialogResult>();
            var message = new ShowUIProgressOnPlayerActionMsg("Inspect", InspectDuration, source);
            Publisher.ForPlayerAction(message);
            await source.Task;
        }

        private async UniTask OnCompleteInspect()
        {
            Log.Debug("OnComplete Inspect");
            Interactable.SetInspectState(EInspectState.Inspected);
            await ShowLootTipAfterInspect();
        }

        private async UniTask ShowLootTipAfterInspect()
        {
            var lootData = Room.GetLoot(InteractableId);

            if (lootData == null)
                throw new Exception("ShowLootTipAfterInspect - no loot data");

            var hasLoot = Room.HasLoot(InteractableId);

            var tipLocalizationId = hasLoot
                ? _inspectSystemTipData.GetRandomTip(InspectSystemTipType.HasLoot)
                : _inspectSystemTipData.GetRandomTip(InspectSystemTipType.NoLoot);

            var tip = LocalizationProvider.Localize(tipLocalizationId, ETable.SmallPhrase);

            var source = new UniTaskCompletionSource<EDialogResult>();

            IUIViewerMsg msg = hasLoot
                ? new ShowHasLootWindowMsg(LocalizedName, tip, lootData, source)
                : new ShowNoLootWindowMsg(LocalizedName.ToUpper(), tip, source);

            try
            {
                Log.Debug(hasLoot
                    ? "<color=green>Inspect - has loot</color>"
                    : "<color=red>Inspect - no loot</color>");

                Publisher.ForUIViewer(msg);
                var result = await source.Task;

                if (result == EDialogResult.Search)
                {
                    Publisher.ForGameManager(new SpendEnergyMsg(1));
                    await StartSearch();
                }
                else if (result == EDialogResult.Close)
                {
                    Log.Debug("ShowLootTipAfterInspect - CLOSE");
                }
                else
                {
                    Log.Warn("Unhandled result: " + result);
                }
            }
            finally
            {
                source?.TrySetCanceled();
            }
        }

        private async UniTask OnStartSearch()
        {
            Log.Debug("OnStart Search");
            // anim hero // await progress bar
            var source = new UniTaskCompletionSource<EDialogResult>();
            Publisher.ForPlayerAction(new ShowUIProgressOnPlayerActionMsg("Search", SearchDuration, source));
            await source.Task;
        }

        private async UniTask OnCompleteSearch()
        {
            Log.Debug("OnComplete Search");
            Interactable.SetInspectState(EInspectState.Searched);
            await ShowLootTipAfterSearch();
        }

        private async UniTask ShowLootTipAfterSearch()
        {
            Log.Debug("ShowLootTipAfterSearch - wait callback from tip ");

            var source = new UniTaskCompletionSource<EDialogResult>();
            var lootData = Room.GetLoot(InteractableId);
            var message = new ShowLootWindowMsg(LocalizedName, lootData, source);

            try
            {
                Publisher.ForUIViewer(message);

                var result = await source.Task;
                source = null;

                if (result == EDialogResult.TakeAll)
                {
                    Log.Debug("ShowLootTipAfterSearch - TAKE ALL");
                    await OnTakeAllLoot(lootData);
                }
                else if (result == EDialogResult.Close)
                {
                    Log.Debug("ShowLootTipAfterSearch - CLOSE");
                }
                else Log.Warn("Unhandled result: " + result);
            }
            finally
            {
                source?.TrySetCanceled();
            }
        }

        private async UniTask OnTakeAllLoot(InspectableData lootData)
        {
            Log.Debug("OnTakeAllLoot");

            Interactable.CanInteract = false;
            Publisher.ForGameManager(new TakeRoomLootMsg(lootData));

            await UniTask.Yield();
        }
    }
}
