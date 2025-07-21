using System.Collections.Generic;
using System.Linq;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.SortMbDelete;
using _StoryGame.Game.Managers.Condition;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.todecor
{
    [SelectionBase]
    [RequireComponent(typeof(Collider))]
    public sealed class NewInteractable : AInteractable<UseSystem>
    {
        [SerializeField] private EMainInteractableType interactableType;
        [SerializeField] private EInteractableState defaultState = EInteractableState.NotSet;
        [SerializeField] private bool disableColliders;
        [SerializeField] private ConditionEffectData conditionEffectVo;

        public override EInteractableType InteractableType => (EInteractableType)interactableType;

        private bool _isInitialized;
        private EInteractableState _currentState;
        private Collider[] _colliders;
        private ConditionChecker _conditionChecker;
        private List<IPassiveDecorator> _passiveDecorators = new();
        private List<IActiveDecorator> _activeDecorators = new();

        [Inject]
        private void Construct(ConditionChecker conditionChecker) => _conditionChecker = conditionChecker;

        protected override void OnAwake()
        {
            if (Equals(conditionEffectVo, default(ConditionEffectData)))
            {
                LOG.Error("ConditionEffectData is null on " + gameObject.name);
                enabled = false;
                return;
            }

            _colliders = GetComponents<Collider>();

            _currentState = defaultState;
            SetState(_currentState);

            CollectDecorators();
        }

        protected override void OnStart()
        {
            UpdatePassiveState();
            _isInitialized = true;
        }

        protected override void Enable()
        {
            if (_isInitialized)
                UpdatePassiveState();
        }

        private void UpdatePassiveState()
        {
            LOG.Warn("--- UpdatePassiveState called for " + gameObject.name);

            IsBlocked = false;

            UpdateState();
            ProcessPassiveDecorators();
            UpdateColliders();
        }

        private void ProcessPassiveDecorators()
        {
            foreach (var decorator in _passiveDecorators)
            {
                if (!decorator.IsEnabled)
                    continue;

                LOG.Warn($"- UpdatePassiveState: decorator={decorator.GetType().Name} / priority={decorator.Priority}");
                decorator.ProcessPassive(this).Forget();
            }
        }

        private void UpdateState()
        {
            var state = _conditionChecker.GetConditionState(conditionEffectVo.condition);

            if (conditionEffectVo.isInverse)
                state = !state;

            SetState(state ? EInteractableState.On : EInteractableState.Off);
        }

        public override async UniTask InteractAsync(ICharacter character)
        {
            if (IsBlocked)
                return;

            await ProcessActiveDecorators();
        }

        private async UniTask ProcessActiveDecorators()
        {
            foreach (var decorator in _activeDecorators)
            {
                if (!decorator.IsEnabled)
                    continue;

                LOG.Warn($"- ActiveDecorator: decorator={decorator.GetType().Name} / priority={decorator.Priority}");
                var result = await decorator.ProcessActive(this);
                if (!result)
                {
                    // Показать сообщение (например, "Нужен лом"), остановить
                    return;
                }
            }
        }

        protected override void UpdateColliders()
        {
            var result = !IsBlocked && (!disableColliders || CurrentState != EInteractableState.Off);

            LOG.Warn("Colliders state to: " + result);
            foreach (var col in _colliders)
                col.enabled = result;
        }

        public void SetBlocked(bool blocked)
        {
            IsBlocked = blocked;
            UpdateColliders();
        }

        private void CollectDecorators()
        {
            _passiveDecorators = GetComponents<IPassiveDecorator>()
                .OrderByDescending(decorator => decorator.Priority)
                .ToList();

            _activeDecorators = GetComponents<IActiveDecorator>()
                .OrderByDescending(decorator => decorator.Priority)
                .ToList();

            foreach (var passive in _passiveDecorators)
                Resolver.Inject(passive);

            foreach (var active in _activeDecorators)
                Resolver.Inject(active);
        }
    }
}
