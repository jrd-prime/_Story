using System;
using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.Loot;
using _StoryGame.Game.UI.Impls.Viewer.Layers;
using _StoryGame.Game.UI.Impls.WorldUI;
using _StoryGame.Infrastructure.Logging;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;

namespace _StoryGame.Game.Interactables.Inspect
{
    public enum DialogResult
    {
        Close,
        TakeAll,
        Search
    }

    public sealed class InspectSystem : IInteractableSystem
    {
        private const float InspectDuration = 2f;
        private const float SearchDuration = 2f;
        private Inspectable _inspectable;

        private readonly ILootSystem _lootSystem;
        private readonly IJLog _log;
        private readonly IPublisher<IUIViewerMessage> _uiViewerMsgPub;

        public InspectSystem(ILootSystem lootSystem, IJLog log, IPublisher<IUIViewerMessage> uiViewerMsgPub,
            IPublisher<ShowPlayerActionProgressMsg> showPlayerActionProgressMsgPub)
        {
            _lootSystem = lootSystem;
            _log = log;
            _uiViewerMsgPub = uiViewerMsgPub;
            _showPlayerActionProgressMsgPub = showPlayerActionProgressMsgPub;
        }

        public async UniTask<bool> Process(Inspectable inspectable)
        {
            _inspectable = inspectable;

            _lootSystem.GenerateLootFor(_inspectable); // generate loot if not generated 

            var inspectState = inspectable.InspectState;

            var result = inspectState switch
            {
                EInspectState.NotInspected => await StartInspect(),
                EInspectState.Inspected => await Inspected(),
                EInspectState.Searched => await Searched(),
                _ => false
            };

            await OnInteractionComplete();

            Debug.Log($"Interact result: {result}");

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

            var source = new UniTaskCompletionSource<DialogResult>();
            _showPlayerActionProgressMsgPub.Publish(new ShowPlayerActionProgressMsg("Inspect", InspectDuration,
                source));

            await source.Task;
        }

        private async UniTask OnCompleteInspect()
        {
            _log.Debug("OnComplete Inspect");
            _inspectable.SetInspectState(EInspectState.Inspected);
            await ShowLootTipAfterInspect();
        }


        bool takeAll = true;
        bool search = true;
        private readonly IPublisher<ShowPlayerActionProgressMsg> _showPlayerActionProgressMsgPub;

        private async UniTask ShowLootTipAfterInspect()
        {
            if (_lootSystem.HasLoot(_inspectable.Id))
            {
                _log.Debug("<color=green>Inspect - has loot</color>");

                var source = new UniTaskCompletionSource<DialogResult>();
                var lootData = _lootSystem.GetGeneratedLoot(_inspectable.Id);
                var message = new ShowHasLootWindowMsg(lootData, source);

                try
                {
                    _uiViewerMsgPub.Publish(message);

                    var result = await source.Task;
                    source = null;

                    if (result == DialogResult.Search)
                    {
                        await OnStartSearch();
                    }
                    else if (result == DialogResult.Close)
                    {
                        Debug.Log("ShowLootTipAfterInspect - CLOSE");
                    }
                }
                finally
                {
                    source?.TrySetCanceled();
                }
            }
            else
            {
                _log.Debug("<color=red>Inspect - no loot</color>");

                var source = new UniTaskCompletionSource<DialogResult>();
                var message = new ShowNoLootWindowMsg(source);
                _uiViewerMsgPub.Publish(message);
                try
                {
                    _uiViewerMsgPub.Publish(message);

                    var result = await source.Task;
                    source = null;

                    if (result == DialogResult.Close)
                    {
                        Debug.Log("ShowLootTipAfterInspect - CLOSE");
                    }
                }
                finally
                {
                    source?.TrySetCanceled();
                }
            }
        }

        private async UniTask OnStartSearch()
        {
            _log.Debug("OnStart Search");
            // anim hero // await progress bar
            var source = new UniTaskCompletionSource<DialogResult>();
            _showPlayerActionProgressMsgPub.Publish(new ShowPlayerActionProgressMsg("Search", InspectDuration,
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
            Debug.Log("ShowLootTipAfterSearch - wait callback from tip . Take all: " + takeAll);

            if (takeAll)
            {
                Debug.Log("ShowLootTipAfterSearch - TAKE ALL");
            }
            else
            {
                Debug.Log("ShowLootTipAfterSearch - CLOSE");
            }

            // add percent to room loot

            await UniTask.Yield();
        }
    }

    public record ShowHasLootWindowMsg(
        GeneratedLootData LootData,
        UniTaskCompletionSource<DialogResult> CompletionSource
    ) : IUIViewerMessage
    {
        public string Name { get; } = nameof(ShowHasLootWindowMsg);
        public GeneratedLootData LootData { get; } = LootData;
        public UniTaskCompletionSource<DialogResult> CompletionSource { get; } = CompletionSource;
        public FloatingWindowType WindowType => FloatingWindowType.HasLoot;
    }

    public record ShowNoLootWindowMsg(
        UniTaskCompletionSource<DialogResult> CompletionSource
    ) : IUIViewerMessage
    {
        public string Name { get; } = nameof(ShowHasLootWindowMsg);
        public UniTaskCompletionSource<DialogResult> CompletionSource { get; } = CompletionSource;
        public FloatingWindowType WindowType => FloatingWindowType.NoLoot;
    }
}
