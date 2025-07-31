using System;

namespace _StoryGame.Game.Managers.Condition
{
    [Serializable]
    public struct OneOfItemCondition
    {
        public string thoughtKey;
        public ItemCondition[] items;
    }
}
