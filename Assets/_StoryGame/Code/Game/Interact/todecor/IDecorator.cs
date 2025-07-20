using _StoryGame.Core.Interact.Interactables;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.todecor
{
    public interface IDecorator
    {
        int Priority { get; }
        bool IsEnabled { get; }
    }

    public interface IActiveDecorator : IDecorator
    {
        UniTask<bool> ProcessActive(IInteractable interactable);
    }

    public interface IPassiveDecorator : IDecorator
    {
        void ProcessPassive(IInteractable interactable);
    }
}
