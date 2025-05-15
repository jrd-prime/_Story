using System;
using _StoryGame.Core.Animations.Interfaces;
using _StoryGame.Core.Character.Player.Interfaces;
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
            var animator = _player.Animator as Animator;
            if (!animator) throw new NullReferenceException("Animator is null");
            animator.SetTrigger(triggerName);
            animator.WaitForAnimationCompleteAsync(animationStateName, onAnimationComplete).Forget();
        }
    }
}
