using System.Text;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Game.Interact.SortMbDelete.InteractablesSORT;
using _StoryGame.Game.Managers.Condition;
using _StoryGame.Game.Movement;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.SortMbDelete.Toggle.Strategies
{
    public class ToggleResponderStrategy : IToggleSystemStrategy
    {
        public string Name => nameof(ToggleResponderStrategy);

        private ESwitchState _switchState;

        private readonly InteractSystemDepFlyweight _dep;
        private readonly ConditionChecker _conditionChecker;

        public ToggleResponderStrategy(InteractSystemDepFlyweight dep, ConditionChecker conditionChecker) =>
            (_dep, _conditionChecker) = (dep, conditionChecker);

        public async UniTask<bool> ExecuteAsync(IToggleable interactable)
        {
            var result = _conditionChecker.GetKeysUnfulfilledConditions(interactable.ConditionsData);

            if (result.Success)
            {
                await UniTask.Yield();
                _dep.Publisher.ForInteractProcessor(new InteractRequestMsg(interactable));
            }
            else
            {
                var localizedThoughtsBuilder = new StringBuilder();

                foreach (var thoughtKey in result.Toughts)
                    localizedThoughtsBuilder.AppendLine("Line / " + _dep.L10n.Localize(thoughtKey, ETable.SmallPhrase));

                var thought = new ThoughtDataVo(localizedThoughtsBuilder.ToString());

                _dep.Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));
            }

            return true;
        }

        public void SetState(ESwitchState switchState) => _switchState = switchState;
    }
}
