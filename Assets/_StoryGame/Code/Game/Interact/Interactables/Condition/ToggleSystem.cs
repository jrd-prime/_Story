using System.Text;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Interactables.Unlock;
using _StoryGame.Game.Movement;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Interactables.Condition
{
    public sealed class ToggleSystem : AInteractSystem<IToggleable>
    {
        private readonly ConditionChecker _conditionChecker;

        public ToggleSystem(InteractSystemDepFlyweight dep, ConditionChecker conditionChecker) : base(dep)
        {
            _conditionChecker = conditionChecker;
        }

        protected override async UniTask<bool> OnInteractAsync()
        {
            Dep.Log.Debug("Toggleable:  interact");


            var result = _conditionChecker.GetKeysUnfulfilledConditions(Interactable.ConditionsData);

            if (result.Success)
            {
                await UniTask.Yield();
                Dep.Publisher.ForInteractProcessor(new InteractRequestMsg(Interactable));
            }
            else
            {
                var localizedThoughtsBuilder = new StringBuilder();

                foreach (var thoughtKey in result.Toughts)
                    localizedThoughtsBuilder.AppendLine("Line / " + Dep.L10n.Localize(thoughtKey, ETable.SmallPhrase));

                var thought = new ThoughtDataVo(localizedThoughtsBuilder.ToString());

                Dep.Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));
            }


            return true;
        }
    }
}
