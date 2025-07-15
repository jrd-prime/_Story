using System;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Data.Anim;
using _StoryGame.Game.Anima;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Interactables.Unlock;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _StoryGame.Game.Interact.InteractableNew.Conditional.Switchable
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

        private const float SpeedMul = 5;

        public EGlobalInteractCondition ImpactCondition => impactCondition;
        protected Collider[] _colliders { get; private set; }
        private bool _isInitialized;
        private AnimatorStateInfo animstate;
        private float normalizedTime;
        protected ESwitchState CurrentState { get; private set; } = ESwitchState.Off;

        protected override void OnAwake()
        {
            _colliders = gameObject.GetComponents<Collider>();
        }

        protected override void OnStart()
        {
            if (initState == ESwitchState.NotSet)
                throw new NullReferenceException("Default state is not set");
            if (!animator)
                throw new Exception($"Animation component not found on {name}.");

            if (impactCondition == EGlobalInteractCondition.NotSet)
                throw new NullReferenceException("ImpactCondition is not set. " + name);

            animator.speed = SpeedMul;

            _isInitialized = true;
        }

        protected override void Enable()
        {
            LOG.Warn("Enable: CurrentState > " + CurrentState);
            if (!_isInitialized)
            {
                LOG.Warn("Enable: _isInitialized == false");
                return;
            }

            // restore animation state
            animator.Play(animstate.shortNameHash, 0, normalizedTime);

            var result = ConditionChecker.GetSwitchState(ImpactCondition);

            LOG.Warn("ImpactCondition > " + ImpactCondition + " > result: " + result + " >  current: " +
                     CurrentState);

            if (result == CurrentState)
                return;

            SetCurrentStateAsync(result).Forget();
        }

        protected override void Disable()
        {
            LOG.Warn("Disable: CurrentState > " + CurrentState);
        }

        protected async UniTask SetCurrentStateAsync(ESwitchState state)
        {
            animator.speed = 1f;
            var trigger = state == ESwitchState.On ? AnimatorConst.TurnOn : AnimatorConst.TurnOff;
            var animState = state == ESwitchState.On ? AnimatorConst.OnStateName : AnimatorConst.OffStateName;

            animator.SetTrigger(trigger);

            LOG.Warn("SetCurrentStateAsync PRE > " + state + " > " + trigger + " > " + animState);
            var waiter = new AnimatorStateWaiter(animator, animState);
            await UniTask.WaitUntil(waiter.IsAnimationFinished);
            LOG.Warn("SetCurrentStateAsync POST < " + state + " > " + trigger + " > " + animState);

            CurrentState = state;
            
            OnStateChanged(state);

            // save animation state
            animstate = animator.GetCurrentAnimatorStateInfo(0);
            normalizedTime = animstate.normalizedTime;
        }

        protected virtual void OnStateChanged(ESwitchState state){}

        private static ESwitchState GetOppositeState(ESwitchState state) =>
            state == ESwitchState.Off ? ESwitchState.On : ESwitchState.Off;


        public string GetSwitchInteractionQuestionKey()
        {
            return switchQuestion switch
            {
                ESwitchQuestion.OpenClose => CurrentState == ESwitchState.On ? "q_close" : "q_open",
                ESwitchQuestion.TurnOnTurnOff => CurrentState == ESwitchState.On ? "q_turn_off" : "q_turn_on",
                ESwitchQuestion.NotSet => "NOT_SET",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void SwitchState() => SetCurrentStateAsync(GetOppositeState(CurrentState)).Forget();
    }

    internal enum ESwitchQuestion
    {
        NotSet = -1,
        NoQuestion = 0,
        OpenClose = 1,
        TurnOnTurnOff = 2
    }
}
