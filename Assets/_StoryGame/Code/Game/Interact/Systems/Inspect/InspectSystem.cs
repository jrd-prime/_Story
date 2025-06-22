using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Inspect
{
    public sealed class InspectSystem : AInteractSystem<IInspectable>
    {
        private readonly InspectStrategyProvider _strategyProvider;

        public InspectSystem(InteractSystemDepFlyweight dep, InspectStrategyProvider strategyProvider) :
            base(dep) => _strategyProvider = strategyProvider;

        protected override async UniTask<bool> OnInteractAsync()
        {
            var strategy = _strategyProvider.GetStrategy(Interactable.InspectState);
            var result = await strategy.ExecuteAsync(Interactable);

            return result;
        }
    }
}
