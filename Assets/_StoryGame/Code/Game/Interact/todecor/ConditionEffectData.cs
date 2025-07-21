using System;
using _StoryGame.Core.Managers;

namespace _StoryGame.Game.Interact.todecor
{
    [Serializable]
    public struct ConditionEffectData
    {
        public EGlobalCondition condition;
        public bool isInverse;
    }
}
