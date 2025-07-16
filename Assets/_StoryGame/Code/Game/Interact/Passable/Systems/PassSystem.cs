using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Passable.Providers;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Passable.Systems
{
    public sealed class PassSystem : AInteractSystem<IPassable>
    {
        private readonly PassableStrategyProvider _strategyProvider;

        public PassSystem(InteractSystemDepFlyweight dep, PassableStrategyProvider strategyProvider) : base(dep) =>
            _strategyProvider = strategyProvider;

        protected override UniTask<bool> OnInteractAsync()
        {
            var strategy = _strategyProvider.GetStrategy(Interactable);
            Dep.Publisher.ForUIViewer(new CurrentOperationMsg(strategy.Name));
            return strategy.ExecuteAsync(Interactable);
        }
    }
}
