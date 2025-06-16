using System;
using _StoryGame.Core.Interfaces.Publisher;
using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Game.Interactables.Data;
using _StoryGame.Game.Interactables.Interfaces;
using _StoryGame.Game.Loot.Impls;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.Localization;
using _StoryGame.Infrastructure.Logging;
using _StoryGame.Infrastructure.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interactables.Impls.Inspect
{
    //TODO NEED REFACTOR (state machine etc)
    public sealed class InspectSystem : IInteractableSystem
    {
        private const float InspectDuration = 2f;
        private const float SearchDuration = 2f;

        private IInspectable _inspectable;
        private string _objName;
        private IRoom _room;
        private string _inspectableId;

        private readonly InspectSystemTipData _settings;
        private readonly ILocalizationProvider _localizationProvider;
        private readonly IJLog _log;
        private readonly IJPublisher _publisher;

        public InspectSystem(
            IJPublisher publisher,
            IJLog log,
            ILocalizationProvider localizationProvider,
            ISettingsProvider settingsProvider)
        {
            _publisher = publisher;
            _log = log;
            _localizationProvider = localizationProvider;
            _settings = settingsProvider.GetSettings<InspectSystemTipData>();
        }

        public async UniTask<bool> Process(IInspectable inspectable)
        {
            _inspectable = inspectable;
            _inspectableId = inspectable.Id;
            _room = inspectable.Room;

            _objName = _localizationProvider.Localize(_inspectable.LocalizationKey, ETable.Words);

            var inspectState = inspectable.InspectState;
            var result = inspectState switch
            {
                EInspectState.NotInspected => await StartInspect(),
                EInspectState.Inspected => await Inspected(),
                EInspectState.Searched => await Searched(),
                _ => false
            };

            await OnInteractionComplete();

            Debug.Log($"Interact <color=green>END</color>. Result: {result}");
            return result;
        }

        private async UniTask<bool> StartInspect()
        {
            _log.Debug("<color=green>Start Inspect</color>".ToUpper());
            await OnStartInspect();
            await Inspected();
            return true;
        }

        private async UniTask<bool> Inspected()
        {
            _log.Debug("<color=green>Inspected</color>".ToUpper());
            await OnCompleteInspect();
            return true;
        }

        private async UniTask<bool> StartSearch()
        {
            _log.Debug("<color=green>Start Search</color>".ToUpper());
            await OnStartSearch();
            await Searched();
            return true;
        }

        private async UniTask<bool> Searched()
        {
            _log.Debug("<color=green>Searched</color>".ToUpper());
            await OnCompleteSearch();
            return true;
        }

        private async UniTask OnInteractionComplete()
        {
            _log.Debug("--- OnInteractionComplete");
            await UniTask.Yield();
        }

        private async UniTask OnStartInspect()
        {
            // anim hero // await progress bar
            _log.Debug("OnStart Inspect");

            var source = new UniTaskCompletionSource<EDialogResult>();
            var message = new ShowUIProgressOnPlayerActionMsg("Inspect", InspectDuration, source);
            _publisher.ForPlayerAction(message);
            await source.Task;
        }

        private async UniTask OnCompleteInspect()
        {
            _log.Debug("OnComplete Inspect");
            _inspectable.SetInspectState(EInspectState.Inspected);
            await ShowLootTipAfterInspect();
        }

        private async UniTask ShowLootTipAfterInspect()
        {
            var lootData = _room.GetLoot(_inspectableId);

            if (lootData == null)
                throw new Exception("ShowLootTipAfterInspect - no loot data");

            var hasLoot = _room.HasLoot(_inspectableId);

            var tipLocalizationId = hasLoot
                ? _settings.GetRandomTip(InspectSystemTipType.HasLoot)
                : _settings.GetRandomTip(InspectSystemTipType.NoLoot);

            var tip = _localizationProvider.Localize(tipLocalizationId, ETable.SmallPhrase);

            var source = new UniTaskCompletionSource<EDialogResult>();

            IUIViewerMsg msg = hasLoot
                ? new ShowHasLootWindowMsg(_objName, tip, lootData, source)
                : new ShowNoLootWindowMsg(_objName.ToUpper(), tip, source);

            try
            {
                _log.Debug(hasLoot
                    ? "<color=green>Inspect - has loot</color>"
                    : "<color=red>Inspect - no loot</color>");

                _publisher.ForUIViewer(msg);
                var result = await source.Task;

                if (result == EDialogResult.Search)
                {
                    _publisher.ForGameManager(new SpendEnergyMsg(1));
                    await StartSearch();
                }
                else if (result == EDialogResult.Close)
                {
                    Debug.Log("ShowLootTipAfterInspect - CLOSE");
                }
                else
                {
                    _log.Warn("Unhandled result: " + result);
                }
            }
            finally
            {
                source?.TrySetCanceled();
            }
        }

        private async UniTask OnStartSearch()
        {
            _log.Debug("OnStart Search");
            // anim hero // await progress bar
            var source = new UniTaskCompletionSource<EDialogResult>();
            _publisher.ForPlayerAction(new ShowUIProgressOnPlayerActionMsg("Search", SearchDuration, source));
            await source.Task;
        }

        private async UniTask OnCompleteSearch()
        {
            _log.Debug("OnComplete Search");
            _inspectable.SetInspectState(EInspectState.Searched);
            await ShowLootTipAfterSearch();
        }

        private async UniTask ShowLootTipAfterSearch()
        {
            Debug.Log("ShowLootTipAfterSearch - wait callback from tip ");

            var source = new UniTaskCompletionSource<EDialogResult>();
            var lootData = _room.GetLoot(_inspectableId);
            var message = new ShowLootWindowMsg(_objName, lootData, source);

            try
            {
                _publisher.ForUIViewer(message);

                var result = await source.Task;
                source = null;

                if (result == EDialogResult.TakeAll)
                {
                    Debug.Log("ShowLootTipAfterSearch - TAKE ALL");
                    await OnTakeAllLoot(lootData);
                }
                else if (result == EDialogResult.Close)
                {
                    Debug.Log("ShowLootTipAfterSearch - CLOSE");
                }
                else _log.Warn("Unhandled result: " + result);
            }
            finally
            {
                source?.TrySetCanceled();
            }
        }

        private async UniTask OnTakeAllLoot(InspectableData lootData)
        {
            _log.Debug("OnTakeAllLoot");

            _inspectable.CanInteract = false;
            _publisher.ForGameManager(new TakeRoomLootMsg(lootData));

            await UniTask.Yield();
        }
    }
}
