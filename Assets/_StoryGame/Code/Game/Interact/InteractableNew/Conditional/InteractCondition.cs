using System;
using _StoryGame.Game.Interact.Interactables.Unlock;
using UnityEngine;

namespace _StoryGame.Game.Interact.InteractableNew.Conditional
{
    [Serializable]
    public struct InteractCondition
    {
        [Range(1, 100)] public int queueIndex;
        public string thoughtKey;
        public EInteractConditionType type;
    }
}
