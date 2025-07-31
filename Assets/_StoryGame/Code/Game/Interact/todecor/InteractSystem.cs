using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.todecor
{
    public class InteractSystem : IInteractableSystem
    {
        public UniTask<bool> Process(IInteractable interactable)
        {
            return UniTask.FromResult(true);
        }
    }
}
