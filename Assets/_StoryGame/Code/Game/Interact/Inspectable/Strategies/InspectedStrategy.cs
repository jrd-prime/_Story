using System;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data.Loot;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.Movement;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Inspectable.Strategies
{
    /// <summary>
    /// Объект исследован. Показываем подсказку есть ли лут
    /// </summary>
    public sealed class InspectedStrategy : IInspectSystemStrategy
    {
        public string Name => nameof(InspectedStrategy);

        private readonly InteractSystemDepFlyweight _dep;
        private IInspectable _inspectable;
        private string _objLocalizedName;
        private readonly DialogResultHandler _dialogResultHandler;

        public InspectedStrategy(InteractSystemDepFlyweight dep)
        {
            _dep = dep;
            _dialogResultHandler = new DialogResultHandler(_dep.Log);

            _dialogResultHandler.AddCallback(EDialogResult.Search, OnSearchAction);
            _dialogResultHandler.AddCallback(EDialogResult.Close, OnCloseAction);
        }

        public async UniTask<bool> ExecuteAsync(IInspectable inspectable)
        {
            _inspectable = inspectable;
            _objLocalizedName = _dep.L10n.Localize(_inspectable.LocalizationKey, ETable.Words, ETextTransform.Upper);

            return await ShowLootTip();
        }

        private async UniTask<bool> ShowLootTip()
        {
            var loot = _inspectable.Loot ?? throw new Exception("ShowLootTip - no loot data");

            PreparedObjLootData objLootData = _dep.LootGenerator.GenerateLootData(_inspectable);

            var hasLoot = _inspectable.HasLoot();

            var tipLocalizationId = hasLoot
                ? _dep.InteractableSystemTipData.GetRandomTip(EInteractableSystemTip.InspHasLoot)
                : _dep.InteractableSystemTipData.GetRandomTip(EInteractableSystemTip.InspNoLoot);

            var tip = _dep.L10n.Localize(tipLocalizationId, ETable.SmallPhrase);

            var source = new UniTaskCompletionSource<EDialogResult>();

            IUIViewerMsg msg = hasLoot
                ? new ShowHasLootWindowMsg(_objLocalizedName, tip, objLootData, source)
                : new ShowNoLootWindowMsg(_objLocalizedName.ToUpper(), tip, source);

            try
            {
                _dep.Log.Debug(hasLoot
                    ? "<color=green>Inspect - has loot</color>"
                    : "<color=red>Inspect - no loot</color>");

                _dep.Publisher.ForUIViewer(msg);
                var result = await source.Task;

                _dialogResultHandler.HandleResultAsync(result);
            }
            finally
            {
                source?.TrySetCanceled();
            }

            return true;
        }

        private UniTask OnCloseAction()  => UniTask.CompletedTask;

        private async UniTask OnSearchAction()
        {
            _dep.Publisher.ForGameManager(new SpendEnergyRequestMsg(1));
            _inspectable.SetInspectState(EInspectState.Searched);
            await UniTask.Yield();
            _dep.Publisher.ForInteractProcessor(new InteractRequestMsg(_inspectable));
        }
    }
}
