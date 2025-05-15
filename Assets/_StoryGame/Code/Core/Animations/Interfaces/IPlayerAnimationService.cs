using System;

namespace _StoryGame.Core.Animations.Interfaces
{
    public interface IPlayerAnimationService
    {
        void AnimateWithTrigger(string triggerName, string animationStateName, Action onAnimationComplete);
    }
}
