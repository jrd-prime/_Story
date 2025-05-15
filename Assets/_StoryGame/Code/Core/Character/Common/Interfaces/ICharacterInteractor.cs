using System;

namespace _StoryGame.Core.Character.Common.Interfaces
{
    public interface ICharacterInteractor
    {
        void AnimateWithTrigger(string triggerName, string animationStateName, Action onAnimationComplete);
    }
}
