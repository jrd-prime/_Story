using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.SortMbDelete.InteractablesSORT;
using _StoryGame.Game.Interact.SortMbDelete.Toggle;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.SortMbDelete
{
    public sealed class ToggleSystem : AInteractSystem<IToggleable>
    {
        private readonly ToggleStrategyProvider _strategyProvider;

        public ToggleSystem(InteractSystemDepFlyweight dep, ToggleStrategyProvider strategyProvider) : base(dep)
            => _strategyProvider = strategyProvider;

        protected override async UniTask<bool> OnInteractAsync()
        {
            var strategy = _strategyProvider.GetStrategy(Interactable.ToggleType, Interactable.SwitchState);

            Dep.Publisher.ForUIViewer(new CurrentOperationMsg(strategy.Name));

            return await strategy.ExecuteAsync(Interactable);
        }
    }
}
