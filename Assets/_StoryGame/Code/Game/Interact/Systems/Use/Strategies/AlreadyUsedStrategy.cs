using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Use.Strategies
{
    public class AlreadyUsedStrategy : IUseSystemStrategy
    {
        public string Name => nameof(AlreadyUsedStrategy);

        private readonly InteractSystemDepFlyweight _dep;

        public AlreadyUsedStrategy(InteractSystemDepFlyweight dep) => _dep = dep;

        public UniTask<bool> ExecuteAsync(IUsable interactable)
        {
            _dep.Log.Warn("Interactable is already used");
            return UniTask.FromResult(true);
        }
    }
}
