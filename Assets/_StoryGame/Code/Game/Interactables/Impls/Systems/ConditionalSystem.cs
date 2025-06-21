using System;
using System.Collections.Generic;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Data;
using _StoryGame.Data.Animator;
using _StoryGame.Data.Interactable;
using _StoryGame.Data.Loot;
using _StoryGame.Data.SO.Currency;
using _StoryGame.Game.Interactables.Abstract;
using _StoryGame.Game.Interactables.Impls.ObjTypes;
using _StoryGame.Game.Managers.Game.Messages;
using Cysharp.Threading.Tasks;
using VContainer;

namespace _StoryGame.Game.Interactables.Impls.Systems
{
    public sealed class ConditionalSystem : AInteractableSystem<Conditional>
    {
        private string _lockedThought;
        private string _lootedThought;
        private Conditional _conditional;

        public ConditionalSystem(IObjectResolver resolver) : base(resolver)
        {
        }

        protected override void OnPreProcess(Conditional interactable)
        {
            _conditional = interactable;

            var lockedThoughtKey = _conditional.LockedStateThought.LocalizationKey;
            _lockedThought = LocalizationProvider.Localize(lockedThoughtKey, ETable.SmallPhrase);

            _lootedThought = LocalizationProvider.Localize(
                InteractableSystemTipData.GetRandomTip(EInteractableSystemTip.CondLooted), ETable.SmallPhrase);
        }

        protected override async UniTask<bool> OnProcessAsync()
        {
            var result = _conditional.ConditionalState switch
            {
                EConditionalState.Unknown => throw new Exception("Conditional state is not set on init room! "),
                EConditionalState.Looted => await LootedAsync(),
                EConditionalState.Locked => await Locked(),
                EConditionalState.Unlocked => await Unlocked(),
                _ => false
            };

            Log.Debug($"Interact <color=green>CONDITIONAL INTERACTION END</color>. Result: {result}");
            return result;
        }

        /// <summary>
        /// Если объект уже был залутан
        /// </summary>
        private UniTask<bool> LootedAsync()
        {
            Publisher.ForUIViewer(new CurrentOperationMsg("Looted. Show thought"));
            var thought = new ThoughtDataVo(_lootedThought);
            Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));
            return UniTask.FromResult(true);
        }

        /// <summary>
        /// Если объект в заблокированном состоянии
        /// </summary>
        private UniTask<bool> Locked()
        {
            Publisher.ForUIViewer(new CurrentOperationMsg("Locked. Show thought"));
            var thought = new ThoughtDataVo(_lockedThought);
            Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));
            return UniTask.FromResult(true);
        }

        /// <summary>
        /// Если объект можно "открыть" и залутать
        /// </summary>
        /// <returns></returns>
        private async UniTask<bool> Unlocked()
        {
            Publisher.ForUIViewer(new CurrentOperationMsg("Unlocked. Looting"));

            SendBoolToPlayerAnimator(AnimatorConst.IsGatherHigh, true);
            await ShowOpenTip();
            SendBoolToPlayerAnimator(AnimatorConst.IsGatherHigh, false);

            var source = new UniTaskCompletionSource<EDialogResult>();
            var lootData = new LootData(Room.Id, _conditional.Id, null, _conditional.Loot);
            var message = new DisplayArtefactInfoMsg(lootData, source);
            var inspdata =
                new InspectableData(LocalizationProvider.Localize(_conditional.Loot.LocalizationKey, ETable.Words),
                    new List<LootData>() { lootData });
            try
            {
                Publisher.ForUIViewer(message);

                var result = await source.Task;
                source = null;

                if (result == EDialogResult.Close)
                {
                    Publisher.ForGameManager(new TakeRoomLootMsg(inspdata));
                }
                else Log.Warn("Unhandled result: " + result);
            }
            finally
            {
                source?.TrySetCanceled();
            }

            _conditional.SetConditionalState(EConditionalState.Looted);
            return true;
        }

        private async UniTask ShowOpenTip()
        {
            var source = new UniTaskCompletionSource<EDialogResult>();
            Publisher.ForPlayerOverHeadUI(new DisplayProgressBarMsg("Open", 3, source));
            await source.Task;
        }
    }
}
