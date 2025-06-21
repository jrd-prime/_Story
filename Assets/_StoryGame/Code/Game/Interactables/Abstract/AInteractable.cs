using System;
using _StoryGame.Game.Interactables.Interfaces;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interactables.Abstract
{
    public abstract class AInteractable<TInteractableSystem> : AInteractableBase
        where TInteractableSystem : IInteractableSystem
    {
        protected TInteractableSystem System;

        private void Start()
        {
            System = Resolver.Resolve<TInteractableSystem>();
            if (Equals(System, default(TInteractableSystem)))
                throw new Exception("InteractableSystem is null. " + gameObject.name);
        }
    }
}
