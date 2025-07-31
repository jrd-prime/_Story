using System;
using _StoryGame.Data.SO.Abstract;

namespace _StoryGame.Game.Managers.Condition
{
    [Serializable]
    public struct ItemCondition
    {
        public string thoughtKey;
        public int amount;
        public ACurrencyData currency;
    }
}
