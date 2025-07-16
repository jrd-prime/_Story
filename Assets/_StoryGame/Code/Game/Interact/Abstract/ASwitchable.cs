using System;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Data.Anim;
using _StoryGame.Game.Anima;
using _StoryGame.Game.Interact.enam;
using _StoryGame.Game.Interact.Interactables.Unlock;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _StoryGame.Game.Interact.Abstract
{
    public abstract class ASwitchable<TInteractSystem, TInteractable> : AConditional<TInteractSystem>, ISwitchable
        where TInteractable : class, ISwitchable
        where TInteractSystem : AInteractSystem<TInteractable>
    {
        [Title(nameof(ASwitchable<TInteractSystem, TInteractable>), titleAlignment: TitleAlignments.Centered)]
        [SerializeField]
        private ESwitchState initState = ESwitchState.NotSet;

        [SerializeField] private ESwitchQuestion switchQuestion = ESwitchQuestion.NotSet;
        [SerializeField] private EGlobalInteractCondition impactCondition = EGlobalInteractCondition.NotSet;
        [SerializeField] private Animator animator;
        [SerializeField] private bool disableCollider;

        public EGlobalInteractCondition ImpactCondition => impactCondition;

        protected ESwitchState CurrentState { get; private set; } = ESwitchState.Off;

        private const float SpeedMul = 5;

        private Collider[] _colliders;
        private bool _isInitialized;
        private AnimatorStateInfo _animStateInfo;
        private float _normalizedTime;

        protected override void OnAwake() => _colliders = gameObject.GetComponents<Collider>();

        protected override void OnStart()
        {
            if (initState == ESwitchState.NotSet)
                throw new NullReferenceException("Default state is not set");

            if (!animator)
                throw new Exception($"Animation component not found on {name}.");

            if (impactCondition == EGlobalInteractCondition.NotSet)
                throw new NullReferenceException("ImpactCondition is not set. " + name);

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
            var result = ConditionChecker.GetSwitchState(ImpactCondition);

            LOG.Warn("ImpactCondition > " + ImpactCondition + " > result: " + result + " >  current: " +
                     CurrentState);

            WhatAboutColliders(CurrentState);
            if (result == CurrentState)
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

            var waiter = new AnimatorStateWaiter(animator, animState);

            await UniTask.WaitUntil(waiter.IsAnimationFinished);

            CurrentState = state;

            OnStateChanged(state);

            // save animation state
            _animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            _normalizedTime = _animStateInfo.normalizedTime;
        }

        private void WhatAboutColliders(ESwitchState state)
        {
            if (state == ESwitchState.On)
            {
                SetCollidersEnabled(true);
                return;
            }

            if (state != ESwitchState.Off)
                return;

            SetCollidersEnabled(!disableCollider);
        }

        private void SetCollidersEnabled(bool isEnabled)
        {
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
                ESwitchQuestion.OpenClose => CurrentState == ESwitchState.On ? "q_close" : "q_open",
                ESwitchQuestion.TurnOnTurnOff => CurrentState == ESwitchState.On ? "q_turn_off" : "q_turn_on",
                ESwitchQuestion.NotSet => "NOT_SET",
                ESwitchQuestion.NoQuestion => "",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void SwitchState() => SetCurrentStateAsync(GetOppositeState(CurrentState)).Forget();
    }
}
