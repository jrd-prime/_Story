using System;
using UnityEngine;

namespace _StoryGame.Game.Anima
{
    public sealed class AnimatorStateWaiter
    {
        private readonly Animator _animator;
        private readonly string _stateName;

        public AnimatorStateWaiter(Animator animator, string stateName)
        {
            _animator = animator;
            _stateName = stateName;
        }

        public bool IsAnimationFinished()
        {
            if (!_animator || !_animator.gameObject.activeInHierarchy)
                throw new Exception("Animator or its GameObject is null or inactive.");

            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(_stateName) && stateInfo.normalizedTime >= 1.0f && !_animator.IsInTransition(0);
        }
    }
}
