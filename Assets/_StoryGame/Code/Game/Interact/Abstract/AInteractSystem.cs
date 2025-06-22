using System;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Abstract
{
    public abstract class AInteractSystem<TInteractable> : IInteractableSystem
        where TInteractable : class, IInteractable
    {
        protected readonly InteractSystemDepFlyweight Dep;

        protected TInteractable Interactable;

        protected AInteractSystem(InteractSystemDepFlyweight dep) => Dep = dep;

        public async UniTask<bool> Process(IInteractable interactable)
        {
            Interactable = interactable as TInteractable ??
                           throw new Exception($"Interact is null as {typeof(TInteractable)}.");

            OnPreInteract();

            var result = await OnInteractAsync();

            OnPostInteract();

            return result;
        }

        protected virtual void OnPreInteract()
        {
        }

        protected abstract UniTask<bool> OnInteractAsync();

        protected virtual void OnPostInteract()
        {
        }
    }
}
