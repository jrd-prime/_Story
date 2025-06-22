using _StoryGame.Core.Interact.Interactables;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Core.Interact
{
    public interface IInteractableSystem
    {
        UniTask<bool> Process(IInteractable interactable);
    }
}
