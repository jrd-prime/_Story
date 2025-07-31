using System;
using _StoryGame.Core.Managers;

namespace _StoryGame.Game.Interact.todecor
{
    [Serializable]
    public struct GlobalConditionEffectData
    {
        public EGlobalCondition condition;
        public bool isInverse;
    }
}
