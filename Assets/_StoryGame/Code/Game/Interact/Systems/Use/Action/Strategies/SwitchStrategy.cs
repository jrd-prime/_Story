using System;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Use.Action.Strategies
{
    public class SwitchStrategy : IUseActionStrategy
    {
        public string StrategyName => nameof(SwitchStrategy);

        public SwitchStrategy(InteractSystemDepFlyweight systemDep)
        {
            throw new NotImplementedException();
        }

        public async UniTask<bool> ExecuteAsync(IUsable usable)
        {
            return await Switch();
        }

        private async UniTask<bool> Switch()
        {
            return true;
        }
    }
}
