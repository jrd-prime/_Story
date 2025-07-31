using System;
using _StoryGame.Core.Interact.Interactables;

namespace _StoryGame.Game.Interact.todecor.Abstract
{
    public static class InteractableExtensions
    {
        public static T As<T>(this IInteractable interactable) where T : class, IInteractable
        {
            if (interactable is not T @as)
                throw new Exception($"Cannot cast {interactable} to {typeof(T)}");

            return @as;
        }
    }
}
