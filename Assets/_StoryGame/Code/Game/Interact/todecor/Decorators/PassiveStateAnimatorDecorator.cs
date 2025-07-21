using System;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Data.Anim;
using _StoryGame.Game.Anima;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.todecor
{
    public sealed class PassiveStateAnimatorDecorator : ADecorator, IPassiveDecorator
    {
        [SerializeField] private Animator animator;
        [SerializeField] private int speedForInit = 20;

        public override int Priority => 10;

        [Inject] private IJLog _log;
        private AnimatorStateInfo _animStateInfo;
        private float _normalizedTime;
        private float _initialSpeed = 1f;

        private void Awake()
        {
            if (!animator)
            {
                _log.Error($"Animator not found on {name}");
                enabled = false;
                return;
            }

            _initialSpeed = animator.speed;
        }

        private void OnEnable()
        {
            if (!IsInitialized)
                return;

            RestoreAnimatorState();
        }

        private void Start() => IsInitialized = true;

        public async UniTask<bool> ProcessPassive(IInteractable interactable)
        {
            var isSpeedChanged = false;

            if (!IsInitialized)
            {
                SetAnimatorSpeed(speedForInit);
                isSpeedChanged = true;
            }

            var state = interactable.CurrentState;
            var trigger = state == EInteractableState.On ? AnimatorConst.TurnOn : AnimatorConst.TurnOff;
            var animState = state == EInteractableState.On ? AnimatorConst.OnStateName : AnimatorConst.OffStateName;

            animator.SetTrigger(trigger);

            try
            {
                var waiter = new AnimatorStateWaiter(animator, animState, _log);
                await UniTask.WaitUntil(waiter.IsAnimationFinished);
            }
            catch (Exception e)
            {
                _log.Error(e.ToString());
                return false;
            }
            finally
            {
                StoreAnimatorState();

                if (isSpeedChanged)
                    SetAnimatorSpeed(_initialSpeed);
            }

            return true;
        }

        private void StoreAnimatorState()
        {
            _animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            _normalizedTime = _animStateInfo.normalizedTime;
        }

        private void RestoreAnimatorState()
        {
            if (_animStateInfo.shortNameHash == 0 || _normalizedTime == 0)
                return;

            animator.Play(_animStateInfo.shortNameHash, 0, _normalizedTime);
        }

        private void SetAnimatorSpeed(float speed) => animator.speed = speed;
    }
}
