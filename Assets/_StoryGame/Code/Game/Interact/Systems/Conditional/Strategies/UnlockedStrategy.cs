using _StoryGame.Core.Animations.Messages;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Data.Animator;
using _StoryGame.Data.Loot;
using _StoryGame.Game.Interact.Systems.Inspect;
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
        public string Name => nameof(UnlockStrategy);
        private readonly InteractSystemDepFlyweight _dep;
        private readonly DialogResultHandler _dialogResultHandler;
        private PreparedObjLootData inspdata;
        private IConditional _conditional;
        private PreparedObjLootData objLootData;

        public UnlockStrategy(InteractSystemDepFlyweight dep)
        {
            _dep = dep;
            _dialogResultHandler = new DialogResultHandler(dep.Log);

            _dialogResultHandler.AddCallback(EDialogResult.TakeAll, OnTakeAllAction);
            _dialogResultHandler.AddCallback(EDialogResult.Close, OnCloseAction);
        }

        private void OnTakeAllAction()
        {
            _dep.Publisher.ForUIViewer(new CurrentOperationMsg("ShowLootTipAfterSearch"));
            _conditional.CanInteract = false; // TODO подумать, мб не вырубать, а показывать хинт
            _dep.Publisher.ForGameManager(new TakeRoomLootMsg(objLootData));
        }

        public async UniTask<bool> ExecuteAsync(IConditional interactable)
        {
            _conditional = interactable;
            await ShowOpenProgress();

            // Показываем окно с лутом
            return await ShowLootTipAfterSearch();
        }

        private async UniTask<bool> ShowLootTipAfterSearch()
        {
            _dep.Publisher.ForUIViewer(new CurrentOperationMsg("ShowLootTipAfterSearch"));

            var source = new UniTaskCompletionSource<EDialogResult>();

            objLootData = _dep.LootGenerator.GenerateLootData(_conditional);
            var message = new ShowLootWindowMsg(objLootData, source);

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
