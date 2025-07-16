using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.SortMbDelete.Use;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.SortMbDelete
{
    public sealed class UseSystem : AInteractSystem<IUsable>
    {
        private readonly UseStrategyProvider _strategyProvider;

        public UseSystem(
            InteractSystemDepFlyweight dep,
            UseStrategyProvider strategyProvider
        ) : base(dep) => _strategyProvider = strategyProvider;

        protected override async UniTask<bool> OnInteractAsync()
        {
            var strategy = _strategyProvider.GetStrategy(Interactable.UseState);
            Dep.Publisher.ForUIViewer(new CurrentOperationMsg(strategy.Name));
            var result = await strategy.ExecuteAsync(Interactable);

            return result;
        }
    }
}
