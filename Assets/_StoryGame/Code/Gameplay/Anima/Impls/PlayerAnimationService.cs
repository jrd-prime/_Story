using System;
using _StoryGame.Core.Animations.Interfaces;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Extensions;
using _StoryGame.Gameplay.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Gameplay.Anima.Impls
{
    public sealed class PlayerAnimationService : IPlayerAnimationService
    {
        private readonly IPlayer _player;

        public PlayerAnimationService(IPlayer player) => _player = player;

        public void AnimateWithTrigger(string triggerName, string animationStateName, Action onAnimationComplete)
        {
            _player.CheckOnNull(nameof(PlayerAnimationService));

            var animator = _player.Animator as Animator;

            animator.CheckOnNull(nameof(PlayerAnimationService));

            animator?.SetTrigger(triggerName);
            animator.WaitForAnimationCompleteAsync(animationStateName, onAnimationComplete).Forget();
        }
    }
}
