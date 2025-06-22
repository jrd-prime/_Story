using _StoryGame.Core.Interact;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Conditional
{
    /// <summary>
    /// Система обработки условных (conditional) взаимодействий с объектами.
    /// Выбирает стратегию выполнения в зависимости от текущего состояния объекта (<see cref="IConditional.ConditionalState"/>).
    /// </summary>
    public sealed class ConditionalSystem : AInteractSystem<IConditional>
    {
        private IConditional _conditional;
        private readonly ConditionalStrategyProvider _strategyProvider;

        public ConditionalSystem(InteractSystemDepFlyweight systemDep, ConditionalStrategyProvider strategyProvider)
            : base(systemDep) => _strategyProvider = strategyProvider;

        protected override void OnPreProcess(IConditional interactable) => _conditional = interactable;

        protected override async UniTask<bool> OnProcessAsync()
        {
            var strategy = _strategyProvider.GetStrategy(_conditional.ConditionalState);
            return await strategy.ExecuteAsync(_conditional);
        }
    }
}
