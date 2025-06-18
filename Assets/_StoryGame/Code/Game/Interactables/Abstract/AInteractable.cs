using _StoryGame.Game.Interactables.Interfaces;
using VContainer;

namespace _StoryGame.Game.Interactables.Abstract
{
    public abstract class AInteractable<TInteractableSystem> : AInteractableBase
        where TInteractableSystem : IInteractableSystem
    {
        protected TInteractableSystem System;

        protected sealed override void ResolveSystem(IObjectResolver resolver) =>
            System = resolver.Resolve<TInteractableSystem>();
    }
}
