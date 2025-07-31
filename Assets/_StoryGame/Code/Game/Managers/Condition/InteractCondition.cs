using System;
using _StoryGame.Core.Managers;
using UnityEngine;

namespace _StoryGame.Game.Managers.Condition
{
    [Serializable]
    public struct InteractCondition
    {
        [Range(1, 100)] public int queueIndex;
        public string thoughtKey;
        public EGlobalCondition type;
    }
}
