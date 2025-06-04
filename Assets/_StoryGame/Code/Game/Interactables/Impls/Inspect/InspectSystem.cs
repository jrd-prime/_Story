using System;
using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Game.Interactables.Data;
using _StoryGame.Game.Interactables.Interfaces;
using _StoryGame.Game.Loot.Impls;
using _StoryGame.Game.Managers.Impls;
using _StoryGame.Game.UI.Impls.WorldUI;
using _StoryGame.Game.UI.Messages;
using _StoryGame.Infrastructure.Localization;
using _StoryGame.Infrastructure.Logging;
using _StoryGame.Infrastructure.Settings;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;

namespace _StoryGame.Game.Interactables.Impls.Inspect
{
    //TODO NEED REFACTOR (state machine etc)
    public sealed class InspectSystem : IInteractableSystem
    {
        private const float InspectDuration = 2f;
        private const float SearchDuration = 2f;
        private IInspectable _inspectable;

        private readonly IJLog _log;
        private readonly IPublisher<IUIViewerMessage> _uiViewerMsgPub;
        private readonly IPublisher<ShowPlayerActionProgressMsg> _showPlayerActionProgressMsgPub;
        private IRoom _room;
        private string _inspectableId;
        private readonly InspectSystemTipData _settings;
        private readonly ILocalizationProvider _localizationProvider;
        private string objName;
        private readonly IPublisher<IGameManagerMsg> _gameManagerMsgPub;

        public InspectSystem(
            IJLog log,
            ILocalizationProvider localizationProvider,
            ISettingsProvider settingsProvider,
            IPublisher<IUIViewerMessage> uiViewerMsgPub,
            IPublisher<ShowPlayerActionProgressMsg> showPlayerActionProgressMsgPub,
            IPublisher<IGameManagerMsg> gameManagerMsgPub)
        {
            _log = log;
            _localizationProvider = localizationProvider;
            _settings = settingsProvider.GetSettings<InspectSystemTipData>();
            _uiViewerMsgPub = uiViewerMsgPub;
            _showPlayerActionProgressMsgPub = showPlayerActionProgressMsgPub;
            _gameManagerMsgPub = gameManagerMsgPub;
        }

        public async UniTask<bool> Process(IInspectable inspectable)
        {
            _inspectable = inspectable;
            _inspectableId = inspectable.Id;
            _room = inspectable.Room;

            objName = _localizationProvider.LocalizeWord(_inspectable.LocalizationKey);

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
            var message = new ShowPlayerActionProgressMsg("Inspect", InspectDuration, source);
            _showPlayerActionProgressMsgPub.Publish(message);
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

            var tip = _localizationProvider.LocalizePhrase(tipLocalizationId);

            var source = new UniTaskCompletionSource<EDialogResult>();

            IUIViewerMessage message = hasLoot
                ? new ShowHasLootWindowMsg(objName, tip, lootData, source)
                : new ShowNoLootWindowMsg(objName, tip, source);

            try
            {
                _log.Debug(hasLoot
                    ? "<color=green>Inspect - has loot</color>"
                    : "<color=red>Inspect - no loot</color>");

                _uiViewerMsgPub.Publish(message);
                var result = await source.Task;

                if (result == EDialogResult.Search)
                {
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
            _showPlayerActionProgressMsgPub.Publish(new ShowPlayerActionProgressMsg("Search", SearchDuration,
                source));
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
            var message = new ShowLootWindowMsg(objName, lootData, source);

            try
            {
                _uiViewerMsgPub.Publish(message);

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
            _gameManagerMsgPub.Publish(new TakeRoomLootMsg(lootData));

            await UniTask.Yield();
        }
    }
}
