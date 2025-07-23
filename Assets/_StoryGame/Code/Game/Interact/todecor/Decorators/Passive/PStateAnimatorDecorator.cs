using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Data.Anim;
using _StoryGame.Game.Interact.todecor.Abstract;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interact.todecor.Decorators.Passive
{
    public sealed class PStateAnimatorDecorator : ADecorator, IPassiveDecorator
    {
        [Space(10)] [SerializeField] private Animator animator;

        public override int Priority => 10;

        private void Awake()
        {
            if (animator)
                return;

            LOG.Error($"Animator not found on {name}");
            enabled = false;
        }

        public UniTask<bool> ProcessPassive(IInteractable interactable)
        {
            var animationStateName = interactable.CurrentState == EInteractableState.On
                ? AnimatorConst.OnStateName
                : AnimatorConst.OffStateName;

            LOG.Debug($"Setting animator to {animationStateName} state. Fast. {name}");
            animator.Play(animationStateName, 0, 1f);

            return UniTask.FromResult(true);
        }
    }
}
