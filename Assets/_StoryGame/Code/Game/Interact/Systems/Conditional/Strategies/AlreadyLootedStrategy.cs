using _StoryGame.Core.Interact;
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
        private readonly InteractSystemDepFlyweight _systemDep;

        public AlreadyLootedStrategy(InteractSystemDepFlyweight systemDep) => _systemDep = systemDep;

        public UniTask<bool> ExecuteAsync(IConditional interactable)
        {
            var lootedThought = _systemDep.LocalizationProvider.Localize(
                _systemDep.InteractableSystemTipData.GetRandomTip(EInteractableSystemTip.CondLooted),
                ETable.SmallPhrase);
            _systemDep.Publisher.ForUIViewer(new CurrentOperationMsg("Looted. Show thought"));
            var thought = new ThoughtDataVo(lootedThought);
            _systemDep.Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));
            return UniTask.FromResult(true);
        }
    }
}
