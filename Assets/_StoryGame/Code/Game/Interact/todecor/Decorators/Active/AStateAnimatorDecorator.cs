using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Data.Anim;
using _StoryGame.Game.Anima;
using _StoryGame.Game.Interact.todecor.Abstract;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interact.todecor.Decorators.Active.Active
{
    public sealed class AStateAnimatorDecorator : ADecorator, IActiveDecorator
    {
        [Space(10)] [SerializeField] private Animator animator;

        public override int Priority => 1;

        protected override void InitializeInternal()
        {
            if (animator)
                return;

            Dep.Log.Error($"Animator not found on {name}");
            enabled = false;
        }

        protected override async UniTask<EDecoratorResult> ProcessInternal(IInteractable interactable)
        {
            var animState = interactable.CurrentState == EInteractableState.On
                ? AnimatorConst.OnStateName
                : AnimatorConst.OffStateName;

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animState))
            {
                var trigger = interactable.CurrentState == EInteractableState.On
                    ? AnimatorConst.TurnOn
                    : AnimatorConst.TurnOff;
                Dep.Log.Warn($"Animator is NOT in {animState} state. Animate. {name}");
                animator.SetTrigger(trigger);
                var stateWaiter = new AnimatorStateWaiter(animator, animState, Dep.Log);
                await UniTask.WaitUntil(stateWaiter.IsAnimationFinished);
            }

            return EDecoratorResult.Success;
        }
    }
}
