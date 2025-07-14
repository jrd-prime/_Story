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
    public abstract class ASwitchable<TInteractSystem> : AConditional<TInteractSystem>, ISwitchable
        where TInteractSystem : AInteractSystem<ISwitchable>
    {
        [Title(nameof(ASwitchable<TInteractSystem>), titleAlignment: TitleAlignments.Centered)] [SerializeField]
        private ESwitchState defaultState = ESwitchState.NotSet;

        [SerializeField] private ESwitchInteractionType interactionType = ESwitchInteractionType.NotSet;
        [SerializeField] private EGlobalInteractCondition impactCondition = EGlobalInteractCondition.NotSet;

        [SerializeField] private Animator animator;

        private const float SpeedMul = 5;

        public EGlobalInteractCondition ImpactCondition => impactCondition;
        private Collider[] _colliders;
        private bool _isInitialized;
        protected ESwitchState CurrentState { get; private set; } = ESwitchState.Off;

        protected override void OnAwake()
        {
            if (defaultState == ESwitchState.NotSet)
                throw new NullReferenceException("Default state is not set");
            if (!animator)
                throw new Exception($"Animation component not found on {name}.");

            if (impactCondition == EGlobalInteractCondition.NotSet)
                throw new NullReferenceException("ImpactCondition is not set. " + name);

            _colliders = gameObject.GetComponents<Collider>();

            animator.speed = SpeedMul;

            _isInitialized = true;
        }

        protected async UniTask SetCurrentStateAsync(ESwitchState state)
        {
            if (CurrentState == state)
                return;
            animator.speed = 1f;
            var trigger = state == ESwitchState.On ? AnimatorConst.TurnOn : AnimatorConst.TurnOff;
            var animState = state == ESwitchState.On ? AnimatorConst.OnStateName : AnimatorConst.OffStateName;

            animator.SetTrigger(trigger);

            LOG.Warn("SetCurrentStateAsync PRE > " + state + " > " + trigger + " > " + animState);
            var waiter = new AnimatorStateWaiter(animator, animState);
            await UniTask.WaitUntil(waiter.IsAnimationFinished);
            LOG.Warn("SetCurrentStateAsync POST < " + state + " > " + trigger + " > " + animState);

            CurrentState = state;
        }

        protected override void Enable()
        {
        }

        private static ESwitchState GetOppositeState(ESwitchState state) =>
            state == ESwitchState.Off ? ESwitchState.On : ESwitchState.Off;


        public string GetSwitchInteractionQuestionKey()
        {
            return interactionType switch
            {
                ESwitchInteractionType.OpenClose => CurrentState == ESwitchState.On ? "q_close" : "q_open",
                ESwitchInteractionType.TurnOnTurnOff => CurrentState == ESwitchState.On ? "q_turn_off" : "q_turn_on",
                ESwitchInteractionType.NotSet => "NOT_SET",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void SwitchState() => SetCurrentStateAsync(GetOppositeState(CurrentState)).Forget();
    }

    internal enum ESwitchInteractionType
    {
        NotSet = -1,
        OpenClose = 0,
        TurnOnTurnOff = 1
    }
}
