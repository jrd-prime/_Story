using System;
using _StoryGame.Core.Interact;
using _StoryGame.Game.Interact.Interactables;
using VContainer;

namespace _StoryGame.Game.Interact.Abstract
{
    public abstract class AInteractable<TInteractableSystem> : AInteractableBase
        where TInteractableSystem : IInteractableSystem
    {
        protected TInteractableSystem System;
        protected ConditionChecker ConditionChecker;

        private void Start()
        {
            System = Resolver.Resolve<TInteractableSystem>();
            if (Equals(System, default(TInteractableSystem)))
                throw new Exception("InteractableSystem is null. " + gameObject.name);

            ConditionChecker = Resolver.Resolve<ConditionChecker>();

            OnStart();
        }

        private void OnEnable()
        {
            OnSomeEnable();
        }


        protected virtual void OnStart()
        {
        }

        protected virtual void OnSomeEnable()
        {
        }
    }
}
