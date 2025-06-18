using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interactables.Interfaces
{
    public interface IInteractableSystem
    {
        UniTask<bool> Process(IInteractable interactable);
    }
}
