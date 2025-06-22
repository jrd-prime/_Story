using System;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Use.Action.Strategies
{
    public class PickUpStrategy : IUseActionStrategy
    {
        public string StrategyName => nameof(PickUpStrategy);

        public PickUpStrategy(InteractSystemDepFlyweight systemDep)
        {
            throw new NotImplementedException();
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
