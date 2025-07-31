using _StoryGame.Core.Interact.Interactables;
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
        private readonly ConditionalStrategyProvider _strategyProvider;

        public ConditionalSystem(InteractSystemDepFlyweight dep, ConditionalStrategyProvider strategyProvider)
            : base(dep) => _strategyProvider = strategyProvider;

        protected override async UniTask<bool> OnInteractAsync()
        {
            var strategy = _strategyProvider.GetStrategy(Interactable.ConditionalState);
            return await strategy.ExecuteAsync(Interactable);
        }
    }
}
