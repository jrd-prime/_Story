using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Gameplay.Extensions
{
    public static class AnimatorExtensions
    {
        public static async UniTask WaitForAnimationCompleteAsync(this Animator animator, string animationName,
            Action onComplete)
        {
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName)) await UniTask.Yield();

            while (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
                   animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                await UniTask.Yield();

            onComplete?.Invoke();
        }
    }
}
