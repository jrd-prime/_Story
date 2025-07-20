using System;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Data.Anim;
using _StoryGame.Game.Anima;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.todecor
{
    public sealed class AnimPassiveDecorator : ADecorator, IPassiveDecorator
    {
        [SerializeField] private Animator animator;
        private IJLog _log;
        public override int Priority => 10;

        private AnimatorStateInfo _animStateInfo;
        private float _normalizedTime;

        [Inject]
        private void Construct(IJLog log)
        {
            _log = log;
            
        }

        private void Awake()
        {
            if (!animator)
                _log.Error($"Animator not found on {name}");
        }

        private void OnEnable()
        {
            if (!IsInitialized)
                return;
        }

        private void Start()
        {
            IsInitialized = true;
        }

        public async void ProcessPassive(IInteractable interactable)
        {
            bool isSpeedChanged = false;
            if (!IsInitialized)
            {
                Debug.LogWarning("animator speed decrease");
                animator.speed = 24f;
                isSpeedChanged = true;
            }

            var state = interactable.CurrentState;
            var trigger = state == EInteractableState.On ? AnimatorConst.TurnOn : AnimatorConst.TurnOff;
            var animState = state == EInteractableState.On ? AnimatorConst.OnStateName : AnimatorConst.OffStateName;

            _log.Warn($"ProcessPassive to: trigger={trigger}, animState={animState}");

            animator.SetTrigger(trigger);

            var waiter = new AnimatorStateWaiter(animator, animState);

            await UniTask.WaitUntil(waiter.IsAnimationFinished);

            // save animation state
            _animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            _normalizedTime = _animStateInfo.normalizedTime;

            if (isSpeedChanged)
                animator.speed = 1;
        }
    }
}
