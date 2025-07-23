using System;
using _StoryGame.Core.Animations.Messages;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI;
using _StoryGame.Data.Anim;
using _StoryGame.Game.Interact.SortMbDelete.Toggle.Strategies;
using _StoryGame.Game.Interact.todecor;
using _StoryGame.Game.Interact.todecor.Abstract;
using _StoryGame.Game.Interact.todecor.Decorators.Active;
using _StoryGame.Game.Managers.Condition.Messages;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Switchable.Systems
{
    public class SimpleSwitchSystem : AActiveDecoratorSystem<ASwitcherDecorator>
    {
        private readonly DialogResultHandler _dialogResultHandler;

        public SimpleSwitchSystem(InteractSystemDepFlyweight dep) : base(dep)
        {
            _dialogResultHandler = new DialogResultHandler(dep.Log);
            _dialogResultHandler.AddCallback(EDialogResult.Yes, OnYesActionAsync);
            _dialogResultHandler.AddCallback(EDialogResult.No, OnNoActionAsync);
        }

        private static UniTask OnNoActionAsync() => UniTask.CompletedTask;

        private async UniTask OnYesActionAsync()
        {
            Dep.Publisher.ForGameManager(new SpendEnergyRequestMsg(Interactable.InteractEnergyCost));
            Dep.Publisher.ForConditionRegistry(new SwitchGlobalConditionMsg(Decorator.ImpactOnCondition));
            await UniTask.Yield();

            Interactable.SwitchState();

            AnimPlayerByBoolAsync(AnimatorConst.IsGatherHigh, 2000).Forget();
            await UniTask.Yield();
        }

        private async UniTask AnimPlayerByBoolAsync(string animName, int duration)
        {
            Dep.Publisher.ForPlayerAnimator(new SetBoolMsg(animName, true));
            await UniTask.Delay(duration);
            Dep.Publisher.ForPlayerAnimator(new SetBoolMsg(animName, false));
        }

        private string GetSwitchInteractionQuestionKey()
        {
            var state = Interactable.CurrentState;
            return Decorator.SwitchQuestion switch
            {
                ESwitchQuestion.OpenClose => state == EInteractableState.On ? "q_close" : "q_open",
                ESwitchQuestion.TurnOnTurnOff => state == EInteractableState.On ? "q_turn_off" : "q_turn_on",
                ESwitchQuestion.NotSet => "NOT_SET",
                ESwitchQuestion.NoQuestion => "",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override async UniTask<bool> OnInteractAsync()
        {
            Dep.Log.Warn("SimpleSwitchSystem.OnInteractAsync");

            Dep.Publisher.ForUIViewer(new CurrentOperationMsg("ToggleModifierStrategy"));

            var source = new UniTaskCompletionSource<EDialogResult>();
            var title = Dep.L10n.Localize(Interactable.LocalizationKey, ETable.Words);
            var state = "state";
            var question = Dep.L10n.Localize(GetSwitchInteractionQuestionKey(), ETable.Words);

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

            // {
            //     // If conditions are not fulfilled
            //     var localizedThoughtsBuilder = new StringBuilder();
            //
            //     foreach (var thoughtKey in conditionsResult.Toughts)
            //         localizedThoughtsBuilder.AppendLine("Line / " + Dep.L10n.Localize(thoughtKey, ETable.SmallPhrase));
            //
            //     var thought = new ThoughtDataVo(localizedThoughtsBuilder.ToString());
            //
            //     Dep.Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));
            // }
            //
            // return true;
        }
    }
}
