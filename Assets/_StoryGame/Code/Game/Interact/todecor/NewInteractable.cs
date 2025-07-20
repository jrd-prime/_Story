using System.Collections.Generic;
using System.Linq;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Managers;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.SortMbDelete;
using _StoryGame.Game.Managers.Condition;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.todecor
{
    [SelectionBase]
    public sealed class NewInteractable : AInteractable<UseSystem>
    {
        [SerializeField] private EMainInteractableType interactableType;
        [SerializeField] private EInteractableState defaultState = EInteractableState.NotSet;
        [SerializeField] private bool disableColliders;
        [SerializeField] private EGlobalCondition conditionAffectingState;

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

            // перед выполнением декораторов, надо проверить состояние из кондишен регистра
            CheckState();

            IsBlocked = false;

            foreach (var decorator in _passiveDecorators)
            {
                if (!decorator.IsEnabled)
                    continue;

                LOG.Warn($"- UpdatePassiveState: decorator={decorator.GetType().Name}");
                decorator.ProcessPassive(this);
            }

            UpdateColliders();
        }

        private void CheckState()
        {
            var state = _conditionChecker.GetConditionState(conditionAffectingState);
            SetState(state);
        }

        public override async UniTask InteractAsync(ICharacter character)
        {
            if (IsBlocked)
                return;

            foreach (var decorator in _activeDecorators)
            {
                if (!decorator.IsEnabled)
                    continue;

                LOG.Warn($"- ActiveDecorator: decorator={decorator.GetType().Name}");
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
