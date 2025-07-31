using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.SortMbDelete.Conditional.Strategies
{
    /// <summary>
    /// Implementation of interaction strategy for locked objects.
    /// </summary>
    public sealed class LockedStrategy : IConditionSystemStrategy
    {
        public string Name => nameof(LockedStrategy);
        private readonly InteractSystemDepFlyweight _dep;
        public LockedStrategy(InteractSystemDepFlyweight dep) => _dep = dep;


        public UniTask<bool> ExecuteAsync(IConditional interactable)
        {
            var lockedThoughtKey = interactable.LockedStateThought.LocalizationKey;
            var lockedThought = _dep.L10n.Localize(lockedThoughtKey, ETable.SmallPhrase);

            var thought = new ThoughtDataVo(lockedThought);
            _dep.Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));
            return UniTask.FromResult(true);
        }
    }
}
