using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Data.Interact;
using _StoryGame.Game.Interact.Systems.Inspect;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Conditional.Strategies
{
    /// <summary>
    /// Strategy implementation for handling already looted interactable objects.
    /// </summary>
    public sealed class AlreadyLootedStrategy : IConditionSystemStrategy
    {
        public string StrategyName => nameof(AlreadyLootedStrategy);
        private readonly InteractSystemDepFlyweight _dep;

        public AlreadyLootedStrategy(InteractSystemDepFlyweight dep) => _dep = dep;

        public UniTask<bool> ExecuteAsync(IConditional interactable)
        {
            _dep.Publisher.ForUIViewer(new CurrentOperationMsg(StrategyName));

            var lootedThought = _dep.LocalizationProvider.Localize(
                _dep.InteractableSystemTipData.GetRandomTip(EInteractableSystemTip.CondLooted),
                ETable.SmallPhrase);

            var thought = new ThoughtDataVo(lootedThought);
            _dep.Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));
            return UniTask.FromResult(true);
        }
    }
}
