using System;
using _StoryGame.Core.Managers;

namespace _StoryGame.Game.Managers.Condition
{
    [Serializable]
    public struct BlockingCondition
    {
        public EGlobalCondition type;
        public bool value;
    }
}
