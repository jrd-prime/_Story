using System.Collections.Generic;
using _StoryGame.Core.Animations.Messages;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Data.Animator;
using _StoryGame.Data.Loot;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Conditional.Strategies
{
    // TODO надо отрефакторить
    /// <summary>
    /// Implementation of interaction strategy for unlocking objects.
    /// </summary>
    public sealed class UnlockStrategy : IConditionSystemStrategy
    {
        public string StrategyName => nameof(UnlockStrategy);
        private readonly InteractSystemDepFlyweight _dep;
        private readonly DialogResultHandler _dialogResultHandler;
        private InspectableData inspdata;
        private IConditional _interactable;

        public UnlockStrategy(InteractSystemDepFlyweight dep)
        {
            _dep = dep;
            _dialogResultHandler = new DialogResultHandler(dep.Log);
            _dialogResultHandler.AddCallback(EDialogResult.Close, OnCloseAction);
        }

        public async UniTask<bool> ExecuteAsync(IConditional interactable)
        {
            _interactable = interactable;
            await ShowOpenProgress();

            return await ShowLootDialog();
        }

        private async UniTask<bool> ShowLootDialog()
        {
            var source = new UniTaskCompletionSource<EDialogResult>();
            var lootData = new LootData(_interactable.Room.Id, _interactable.Id, null, _interactable.Loot);
            var message = new DisplayArtefactInfoMsg(lootData, source);
            var locName = _dep.LocalizationProvider.Localize(_interactable.LocalizationKey, ETable.Words);
            inspdata = new InspectableData(locName, new List<LootData>() { lootData });

            try
            {
                _dep.Publisher.ForUIViewer(message);

                var result = await source.Task;
                source = null;

                _dialogResultHandler.HandleResult(result);
            }
            finally
            {
                source?.TrySetCanceled();
            }

            _interactable.SetConditionalState(EConditionalState.Looted);
            return true;
        }

        private void OnCloseAction()
        {
            _dep.Publisher.ForGameManager(new TakeRoomLootMsg(inspdata));
        }

        /// <summary>
        /// Displays the progress bar for "opening" animation.
        /// </summary>
        private async UniTask ShowOpenProgress()
        {
            _dep.Publisher.ForPlayerAnimator(new SetBoolMsg(AnimatorConst.IsGatherHigh, true));

            var source = new UniTaskCompletionSource<EDialogResult>();
            _dep.Publisher.ForPlayerOverHeadUI(new DisplayProgressBarMsg("Open", 3, source));
            await source.Task;

            _dep.Publisher.ForPlayerAnimator(new SetBoolMsg(AnimatorConst.IsGatherHigh, false));
        }
    }
}
