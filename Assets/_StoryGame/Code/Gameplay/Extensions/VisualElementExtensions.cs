using System;
using UnityEngine.UIElements;

namespace _StoryGame.Gameplay.Extensions
{
    public static class VisualElementExtensions
    {
        public static T GetVisualElement<T>(this VisualElement root, string id, string name) where T : VisualElement
        {
            var element = root.Q<T>(id);
            if (element == null)
                throw new NullReferenceException($"Element with ID '{id}' not found in UIDoc on {name}.");

            return element;
        }
    }
}
