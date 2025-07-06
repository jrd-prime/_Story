using System;

namespace _StoryGame.Game.Interact.Interactables.Unlock
{
    [Serializable]
    public struct UnlockConditionData
    {
        public InteractCondition[] conditions;
        public ItemCondition[] requiredItems;
        public OneOfItemCondition oneOfItem;

        public bool HasConditions()
        {
            if (conditions is { Length: > 0 })
                return true;
            if (oneOfItem.items is { Length: > 0 })
                return true;

            return requiredItems is { Length: > 0 };
        }
    }

    [Serializable]
    public struct OneOfItemCondition
    {
        public string thoughtKey;
        public ItemCondition[] items;
    }
}
