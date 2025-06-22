using _StoryGame.Core.Interactables;
using _StoryGame.Game.Interactables.Abstract;
using _StoryGame.Infrastructure.Interactables;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interactables.Impls.Systems.Cond
{
    public sealed class ConditionalSystem : AInteractSystem<IConditional>
    {
        private IConditional _conditional;
        private readonly ConditionalStrategyProvider _strategyProvider;

        public ConditionalSystem(InteracSystemDepFlyweight systemDep, ConditionalStrategyProvider strategyProvider)
            : base(systemDep) => _strategyProvider = strategyProvider;

        protected override void OnPreProcess(IConditional interactable) => _conditional = interactable;

        protected override async UniTask<bool> OnProcessAsync()
        {
            var strategy = _strategyProvider.GetStrategy(_conditional.ConditionalState);
            return await strategy.ExecuteAsync(_conditional);
        }
    }
}
