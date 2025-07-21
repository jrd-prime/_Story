using System;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Managers;
using _StoryGame.Data.Anim;
using _StoryGame.Game.Anima;
using _StoryGame.Game.Interact.Abstract;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _StoryGame.Game.Interact.Switchable.Abstract
{
    public abstract class ASwitchable<TInteractSystem, TInteractable> : AConditional<TInteractSystem>, ISwitchable
        where TInteractable : class, ISwitchable
        where TInteractSystem : AInteractSystem<TInteractable>
    {
        [Title(nameof(ASwitchable<TInteractSystem, TInteractable>), titleAlignment: TitleAlignments.Centered)]
        [SerializeField]
        private ESwitchState initState = ESwitchState.NotSet;

        [SerializeField] private ESwitchQuestion switchQuestion = ESwitchQuestion.NotSet;
        [SerializeField] private EGlobalCondition impactCondition = EGlobalCondition.NotSet;
        [SerializeField] private Animator animator;
        [SerializeField] private bool disableCollider;

        public EGlobalCondition ImpactCondition => impactCondition;

        protected ESwitchState CurrentSwitchState { get; private set; } = ESwitchState.Off;

        private const float SpeedMul = 5;

        private Collider[] _colliders;
        private bool _isInitialized;
        private AnimatorStateInfo _animStateInfo;
        private float _normalizedTime;

        protected override void OnAwake() => _colliders = gameObject.GetComponents<Collider>();

        protected override void OnStart()
        {
            if (initState == ESwitchState.NotSet)
                LOG.Error("Default state is not set");

            if (!animator)
                LOG.Error($"Animation component not found on {name}.");

            if (impactCondition == EGlobalCondition.NotSet)
                LOG.Error("ImpactCondition is not set. " + name);

            animator.speed = SpeedMul;
            InitState();

            _isInitialized = true;
        }

        protected override void Enable()
        {
            if (!_isInitialized)
                return;

            animator.Play(_animStateInfo.shortNameHash, 0, _normalizedTime);

            InitState();
        }

        private void InitState()
        {
            WhatAboutColliders(CurrentSwitchState);

            var result = ConditionChecker.GetSwitchState(ImpactCondition);

            LOG.Warn("ImpactCondition > " + ImpactCondition + " > result: " + result + " >  current: " +
                     CurrentSwitchState);

            if (result == CurrentSwitchState)
                return;

            SetCurrentStateAsync(result).Forget();
        }

        protected async UniTask SetCurrentStateAsync(ESwitchState state)
        {
            WhatAboutColliders(state);


            var trigger = state == ESwitchState.On ? AnimatorConst.TurnOn : AnimatorConst.TurnOff;
            var animState = state == ESwitchState.On ? AnimatorConst.OnStateName : AnimatorConst.OffStateName;

            animator.speed = 1f;
            animator.SetTrigger(trigger);

            var waiter = new AnimatorStateWaiter(animator, animState, LOG);

            await UniTask.WaitUntil(waiter.IsAnimationFinished);

            CurrentSwitchState = state;

            OnStateChanged(state);

            // save animation state
            _animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            _normalizedTime = _animStateInfo.normalizedTime;
        }

        private void WhatAboutColliders(ESwitchState state)
        {
            var isBlocked = ConditionChecker.IsInteractBlocked(ConditionsData.blockingConditions);
            LOG.Warn($"WhatAboutColliders: state={state}, disableCollider={disableCollider}, isBlocked={isBlocked}");

            if (isBlocked)
            {
                SetCollidersEnabled(false); // Блокировка отключает коллайдеры
                return;
            }

            if (state == ESwitchState.On)
            {
                SetCollidersEnabled(true); // При On коллайдеры включены
                return;
            }

            if (state == ESwitchState.Off)
            {
                SetCollidersEnabled(!disableCollider); // При Off зависит от disableCollider
                return;
            }
        }

        private void SetCollidersEnabled(bool isEnabled)
        {
            LOG.Warn("Colliders state to: " + isEnabled);
            foreach (var col in _colliders)
                col.enabled = isEnabled;
        }

        protected virtual void OnStateChanged(ESwitchState state)
        {
        }

        private static ESwitchState GetOppositeState(ESwitchState state) =>
            state == ESwitchState.Off ? ESwitchState.On : ESwitchState.Off;


        public string GetSwitchInteractionQuestionKey()
        {
            return switchQuestion switch
            {
                ESwitchQuestion.OpenClose => CurrentSwitchState == ESwitchState.On ? "q_close" : "q_open",
                ESwitchQuestion.TurnOnTurnOff => CurrentSwitchState == ESwitchState.On ? "q_turn_off" : "q_turn_on",
                ESwitchQuestion.NotSet => "NOT_SET",
                ESwitchQuestion.NoQuestion => "",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void SwitchState() => SetCurrentStateAsync(GetOppositeState(CurrentSwitchState)).Forget();
    }
}
