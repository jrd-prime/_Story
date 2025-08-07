using System;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;
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

        [Inject] protected InteractSystemDepFlyweight Dep;

        public void Initialize()
        {
            InitializeInternal();
            IsInitialized = true;
        }

        private void Start()
        {
            if (Dep == null)
                throw new NullReferenceException("Dep is null.");
        }

        public UniTask<EDecoratorResult> Process(IInteractable interactable)
        {
            if (IsInitialized)
                return ProcessInternal(interactable);

            Dep.Log.Error($"{this} Not Initialized. Call Initialize().");
            enabled = false;
            return UniTask.FromResult(EDecoratorResult.Error);
        }

        protected abstract void InitializeInternal();
        protected abstract UniTask<EDecoratorResult> ProcessInternal(IInteractable interactable);
    }
}
