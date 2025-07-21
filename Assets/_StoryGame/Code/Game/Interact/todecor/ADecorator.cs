using _StoryGame.Core.Interact;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.todecor
{
    public abstract class ADecorator : MonoBehaviour, IDecorator
    {
        [SerializeField] private bool isEnabled = true;
        public abstract int Priority { get; }
        public bool IsEnabled => isEnabled;

        protected bool IsInitialized;
    }
}
