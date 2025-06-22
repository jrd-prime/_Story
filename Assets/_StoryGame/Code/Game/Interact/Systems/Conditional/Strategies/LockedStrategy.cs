using _StoryGame.Core.Interact;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Game.Interact.Systems.Inspect;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Conditional.Strategies
{
    /// <summary>
    /// Если объект в заблокированном состоянии
    /// </summary>
    public sealed class LockedStrategy : IConditionalSystemStrategy
    {
        public string StrategyName => nameof(LockedStrategy);
        private readonly InteractSystemDepFlyweight _systemDep;
        public LockedStrategy(InteractSystemDepFlyweight systemDep) => _systemDep = systemDep;


        public UniTask<bool> ExecuteAsync(IConditional interactable)
        {
            _systemDep.Log.Debug("Conditional = " + interactable);
            var lockedThoughtKey = interactable.LockedStateThought.LocalizationKey;
            var lockedThought = _systemDep.LocalizationProvider.Localize(lockedThoughtKey, ETable.SmallPhrase);

            _systemDep.Publisher.ForUIViewer(new CurrentOperationMsg("Locked. Show thought"));

            var thought = new ThoughtDataVo(lockedThought);
            _systemDep.Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));
            return UniTask.FromResult(true);
        }
    }
}
