using System;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.todecor;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Abstract
{
    public abstract class AActiveDecoratorSystem<TDecorator> where TDecorator : IActiveDecorator
    {
        protected readonly InteractSystemDepFlyweight Dep;
        protected TDecorator Decorator;
        protected IInteractable Interactable;

        protected AActiveDecoratorSystem(InteractSystemDepFlyweight dep) => Dep = dep;

        public async UniTask<bool> Process(TDecorator decorator, IInteractable interactable)
        {
            Dep.Log.Warn("AInteractSystem.ProcessActive: " + decorator);

            if (decorator == null || interactable == null)
                throw new Exception("Decorator or Interactable is null.");

            Decorator = decorator;
            Interactable = interactable;

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
