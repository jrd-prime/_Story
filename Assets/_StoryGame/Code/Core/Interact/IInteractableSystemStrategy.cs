using _StoryGame.Core.Common.Interfaces;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Core.Interact
{
    public interface IInteractableSystemStrategy<in TInteractable> : IStrategy
        where TInteractable : IInteractable
    {
        UniTask<bool> ExecuteAsync(TInteractable interactable);
    }
}
