using _StoryGame.Core.Interact;
using _StoryGame.Game.Interact.Interactables;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Unlockable.Strategies
{
    public sealed class UnlockedDoorStrategy : IUnlockSystemStrategy
    {
        public UnlockedDoorStrategy(InteractSystemDepFlyweight dep)
        {
        }

        public string Name => nameof(LockedDoorStrategy);

        public UniTask<bool> ExecuteAsync(IUnlockable interactable)
        {
            throw new System.NotImplementedException();
        }
    }
}
