using System.Text;
using _StoryGame.Core.Animations.Messages;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Data.Anim;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.SortMbDelete.Toggle.Strategies;
using _StoryGame.Game.Managers.Condition.Messages;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Switchable.Systems
{
    public class SimpleSwitchSystem : AInteractSystem<ISimpleSwitchable>
    {
        private readonly DialogResultHandler _dialogResultHandler;
        private int cost;

        public SimpleSwitchSystem(InteractSystemDepFlyweight dep) : base(dep)
        {
            _dialogResultHandler = new DialogResultHandler(dep.Log);
            _dialogResultHandler.AddCallback(EDialogResult.Yes, OnYesActionAsync);
            _dialogResultHandler.AddCallback(EDialogResult.No, OnNoActionAsync);
        }

        private UniTask OnNoActionAsync() => UniTask.CompletedTask;

        private async UniTask OnYesActionAsync()
        {
            Dep.Publisher.ForGameManager(new SpendEnergyRequestMsg(Interactable.InteractEnergyCost));

            Interactable.SwitchState();

            await AnimPlayerByBoolAsync(AnimatorConst.IsGatherHigh, 2000);

            Dep.Publisher.ForConditionRegistry(new SwitchGlobalConditionMsg(Interactable.ImpactCondition));


            await UniTask.Yield();
        }

        private async UniTask AnimPlayerByBoolAsync(string animName, int duration)
        {
            Dep.Publisher.ForPlayerAnimator(new SetBoolMsg(animName, true));
            await UniTask.Delay(duration);
            Dep.Publisher.ForPlayerAnimator(new SetBoolMsg(animName, false));
        }

        protected override async UniTask<bool> OnInteractAsync()
        {
            Dep.Log.Warn("SimpleSwitchSystem.OnInteractAsync");

            var conditionsResult = Dep.ConditionChecker.CheckConditions(Interactable.ConditionsData);

            if (conditionsResult.Success)
            {
                // If conditions are fulfilled
                Dep.Publisher.ForUIViewer(new CurrentOperationMsg("ToggleModifierStrategy"));

                var source = new UniTaskCompletionSource<EDialogResult>();
                var title = Dep.L10n.Localize(Interactable.LocalizationKey, ETable.Words);
                var state = "state";
                var question = Dep.L10n.Localize(Interactable.GetSwitchInteractionQuestionKey(), ETable.Words);

                var message = new ShowDialogWindowMsg(title, state, question, Interactable.InteractEnergyCost, source);

                try
                {
                    Dep.Publisher.ForUIViewer(message);

                    var result = await source.Task;
                    source = null;

                    await _dialogResultHandler.HandleResultAsync(result);
                }
                finally
                {
                    source?.TrySetCanceled();
                }

                return true;
            }
            else
            {
                // If conditions are not fulfilled
                var localizedThoughtsBuilder = new StringBuilder();

                foreach (var thoughtKey in conditionsResult.Toughts)
                    localizedThoughtsBuilder.AppendLine("Line / " + Dep.L10n.Localize(thoughtKey, ETable.SmallPhrase));

                var thought = new ThoughtDataVo(localizedThoughtsBuilder.ToString());

                Dep.Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));
            }

            return true;
        }
    }
}
