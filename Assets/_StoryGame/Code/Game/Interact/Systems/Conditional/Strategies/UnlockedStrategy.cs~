using System.Collections.Generic;
using _StoryGame.Core.Animations.Messages;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Data;
using _StoryGame.Data.Animator;
using _StoryGame.Data.Interact;
using _StoryGame.Data.Loot;
using _StoryGame.Game.Interact.Systems.Inspect;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Conditional.Strategies
{
    /// <summary>
    /// Implementation of interaction strategy for unlocking objects.
    /// </summary>
    public sealed class UnlockStrategy : IConditionSystemStrategy
    {
        public string StrategyName => nameof(UnlockStrategy);
        private readonly InteractSystemDepFlyweight _dep;

        public UnlockStrategy(InteractSystemDepFlyweight dep) => _dep = dep;

        public async UniTask<bool> ExecuteAsync(IConditional interactable)
        {
            _dep.Publisher.ForUIViewer(new CurrentOperationMsg(StrategyName));

            _dep.Publisher.ForPlayerAnimator(new SetBoolMsg(AnimatorConst.IsGatherHigh, true));

            await ShowOpenTip();
            _dep.Publisher.ForPlayerAnimator(new SetBoolMsg(AnimatorConst.IsGatherHigh, false));

            var source = new UniTaskCompletionSource<EDialogResult>();
            var lootData = new LootData(interactable.Room.Id, interactable.Id, null, interactable.Loot);
            var message = new DisplayArtefactInfoMsg(lootData, source);
            var locName = _dep.LocalizationProvider.Localize(interactable.LocalizationKey, ETable.Words);
            var inspdata = new InspectableData(locName, new List<LootData>() { lootData });
            try
            {
                _dep.Publisher.ForUIViewer(message);

                var result = await source.Task;
                source = null;

                if (result == EDialogResult.Close)
                {
                    _dep.Publisher.ForGameManager(new TakeRoomLootMsg(inspdata));
                }
                else _dep.Log.Warn("Unhandled unexpected result: " + result);
            }
            finally
            {
                source?.TrySetCanceled();
            }

            interactable.SetConditionalState(EConditionalState.Looted);
            return true;
        }

        /// <summary>
        /// Displays the progress bar for "opening" animation.
        /// </summary>
        private async UniTask ShowOpenTip()
        {
            var source = new UniTaskCompletionSource<EDialogResult>();
            _dep.Publisher.ForPlayerOverHeadUI(new DisplayProgressBarMsg("Open", 3, source));
            await source.Task;
        }
    }
}
