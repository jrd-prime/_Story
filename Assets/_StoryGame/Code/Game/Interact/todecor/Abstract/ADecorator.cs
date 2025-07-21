using Sirenix.OdinInspector;
using UnityEngine;

namespace _StoryGame.Game.Interact.todecor.Abstract
{
    public abstract class ADecorator : MonoBehaviour, IDecorator
    {
        [SerializeField] private bool isEnabled = true;
        [ShowInInspector] [ReadOnly] public abstract int Priority { get; }
        public bool IsEnabled => isEnabled;

        protected bool IsInitialized;
    }
}
