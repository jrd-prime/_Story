using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Infrastructure.Interact;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.todecor.Abstract
{
    public abstract class ADecorator : MonoBehaviour, IDecorator
    {
        [SerializeField] private bool isEnabled = true;
        [ShowInInspector] [ReadOnly] public abstract int Priority { get; }
        public bool IsEnabled => isEnabled;

        protected bool IsInitialized;
        protected IJLog LOG;

        [Inject]
        private void Construct(IJLog log)
        {
            LOG = log;
        }
    }
}
