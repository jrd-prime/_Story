using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Data.Anim;
using _StoryGame.Game.Anima;
using _StoryGame.Game.Interact.todecor.Abstract;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.todecor.Decorators.Passive
{
    public sealed class PStateAnimatorDecorator : ADecorator, IPassiveDecorator
    {
        [SerializeField] private Animator animator;

        public override int Priority => 10;

        [Inject] private IJLog _log;
        private AnimatorStateInfo _animStateInfo;
        private float _normalizedTime;
        private string animState;

        private void Awake()
        {
            if (animator)
                return;

            _log.Error($"Animator not found on {name}");
            enabled = false;
        }

        private void OnEnable()
        {
            if (!IsInitialized)
                return;

            RestoreAnimatorState();
        }

        public async UniTask<bool> ProcessPassive(IInteractable interactable)
        {
            Debug.LogWarning($"ProcessPassive for ANIM {IsInitialized} ");
            var isFirstInitialization = !IsInitialized;
            var state = interactable.CurrentState;

            animState = state == EInteractableState.On ? AnimatorConst.OnStateName : AnimatorConst.OffStateName;

            if (isFirstInitialization) // Используем сохраненное значение
            {
                // Мгновенно переключаемся в нужное состояние, в конец анимации
                animator.Play(animState, 0, 1.0f);
                StoreAnimatorState(); // Сохраняем это состояние
                IsInitialized = true; // !!! ВАЖНО: Установить IsInitialized в true после первой инициализации
                return true; // Завершаем ProcessPassive
            }

            var trigger = state == EInteractableState.On ? AnimatorConst.TurnOn : AnimatorConst.TurnOff;

            Debug.LogWarning($"ProcessPassive for ANIM  {state} / {trigger} / {animState}");
            animator.SetTrigger(trigger);

            var waiter = new AnimatorStateWaiter(animator, animState, _log);
            await UniTask.WaitUntil(waiter.IsAnimationFinished);

            StoreAnimatorState();


            return true;
        }

        private void StoreAnimatorState()
        {
            _animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            _normalizedTime = _animStateInfo.normalizedTime;
            Debug.LogWarning($"StoreAnimatorState: " +
                             $"shortNameHash={_animStateInfo.shortNameHash} / normalizedTime={_normalizedTime}");
        }

        private void RestoreAnimatorState()
        {
            Debug.LogWarning($"RestoreAnimatorState: " +
                             $"shortNameHash={_animStateInfo.shortNameHash} / normalizedTime={_normalizedTime}");
            animator.Play(_animStateInfo.shortNameHash, 0, _normalizedTime);
        }

        private void SetAnimatorSpeed(float speed) => animator.speed = speed;
    }
}
