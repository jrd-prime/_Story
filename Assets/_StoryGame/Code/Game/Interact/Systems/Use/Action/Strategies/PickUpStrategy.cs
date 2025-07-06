using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Use.Action.Strategies
{
    public class PickUpStrategy : IUseActionStrategy
    {
        public string Name => nameof(PickUpStrategy);

        public PickUpStrategy(InteractSystemDepFlyweight systemDep)
        {
        }

        public async UniTask<bool> ExecuteAsync(IUsable usable)
        {
            return await PickUp();
        }

        private async UniTask<bool> PickUp()
        {
            return true;
        }
    }
}
