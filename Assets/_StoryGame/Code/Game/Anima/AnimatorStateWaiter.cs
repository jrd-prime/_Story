using _StoryGame.Core.Common.Interfaces;
using UnityEngine;

namespace _StoryGame.Game.Anima
{
    public sealed class AnimatorStateWaiter
    {
        private readonly Animator _animator;
        private readonly string _stateName;
        private readonly IJLog _log;

        public AnimatorStateWaiter(Animator animator, string stateName, IJLog log)
        {
            _animator = animator;
            _stateName = stateName;
            _log = log;
        }

        public bool IsAnimationFinished()
        {
            if (!_animator)
            {
                _log.Error("Animator is null.");
            }

            if (!_animator.gameObject.activeInHierarchy)
            {
                _log.Error("Animator GameObject is inactive. " + _animator.gameObject.name);
            }

            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(_stateName) && stateInfo.normalizedTime >= 1.0f && !_animator.IsInTransition(0);
        }
    }
}
