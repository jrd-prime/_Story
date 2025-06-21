using System;
using System.Collections.Generic;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Data;
using _StoryGame.Data.Animator;
using _StoryGame.Data.Interactable;
using _StoryGame.Data.Loot;
using _StoryGame.Game.Interactables.Abstract;
using _StoryGame.Game.Interactables.Impls.ObjTypes;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
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
            Log.Debug("<color=yellow>Looted. Show thought</color>".ToUpper());
            var thought = new ThoughtDataVo(_lootedThought);
            Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));
            return UniTask.FromResult(true);
        }

        /// <summary>
        /// Если объект в заблокированном состоянии
        /// </summary>
        private UniTask<bool> Locked()
        {
            Log.Debug("<color=yellow>Locked. Show thought</color>".ToUpper());
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
            Log.Debug("<color=yellow>Unlocked</color>".ToUpper());

            SendBoolToPlayerAnimator(AnimatorConst.IsGatherHigh, true);
            await ShowOpenTip();
            SendBoolToPlayerAnimator(AnimatorConst.IsGatherHigh, false);

            // on compl > show artefact info
            // on comp art info > add to journal
            // mb tip


            var aa = new InspectableLootData(Room.Id, _conditional.Id, null, _conditional.Loot);

            var a = new InspectableData("afdasd", new List<InspectableLootData> { aa });
            Publisher.ForGameManager(new TakeRoomLootMsg(a));

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
