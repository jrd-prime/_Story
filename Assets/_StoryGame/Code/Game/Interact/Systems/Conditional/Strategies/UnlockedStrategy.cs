using System.Collections.Generic;
using _StoryGame.Core.Animations.Messages;
using _StoryGame.Core.Interact;
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
    /// Если объект можно "открыть" и залутать
    /// </summary>
    /// <returns></returns>
    public sealed class UnlockStrategy : IConditionalSystemStrategy
    {
        public string StrategyName => nameof(UnlockStrategy);
        private readonly InteractSystemDepFlyweight _systemDep;

        public UnlockStrategy(InteractSystemDepFlyweight systemDep) => _systemDep = systemDep;

        public async UniTask<bool> ExecuteAsync(IConditional interactable)
        {
            _systemDep.Publisher.ForUIViewer(new CurrentOperationMsg("Unlocked. Looting"));

            _systemDep.Publisher.ForPlayerAnimator(new SetBoolMsg(AnimatorConst.IsGatherHigh, true));

            await ShowOpenTip();
            _systemDep.Publisher.ForPlayerAnimator(new SetBoolMsg(AnimatorConst.IsGatherHigh, false));

            var source = new UniTaskCompletionSource<EDialogResult>();
            var lootData = new LootData(interactable.Room.Id, interactable.Id, null, interactable.Loot);
            var message = new DisplayArtefactInfoMsg(lootData, source);
            var locName = _systemDep.LocalizationProvider.Localize(interactable.LocalizationKey, ETable.Words);
            var inspdata = new InspectableData(locName, new List<LootData>() { lootData });
            try
            {
                _systemDep.Publisher.ForUIViewer(message);

                var result = await source.Task;
                source = null;

                if (result == EDialogResult.Close)
                {
                    _systemDep.Publisher.ForGameManager(new TakeRoomLootMsg(inspdata));
                }
                else _systemDep.Log.Warn("Unhandled unexpected result: " + result);
            }
            finally
            {
                source?.TrySetCanceled();
            }

            interactable.SetConditionalState(EConditionalState.Looted);
            return true;
        }

        private async UniTask ShowOpenTip()
        {
            var source = new UniTaskCompletionSource<EDialogResult>();
            _systemDep.Publisher.ForPlayerOverHeadUI(new DisplayProgressBarMsg("Open", 3, source));
            await source.Task;
        }
    }
}
