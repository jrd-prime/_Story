using _StoryGame.Core.Interact.Interactables;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.todecor
{
    public interface IDecorator
    {
        int Priority { get; }
        bool IsEnabled { get; }
        void Initialize();
        UniTask<EDecoratorResult> Process(IInteractable interactable);
    }

    public interface IActiveDecorator : IDecorator
    {
    }

    public interface IPassiveDecorator : IDecorator
    {
    }

    public enum EDecoratorResult
    {
        Ignore = -1,
        Error = 0,
        Success = 1,
        Suspend = 13
    }
}
