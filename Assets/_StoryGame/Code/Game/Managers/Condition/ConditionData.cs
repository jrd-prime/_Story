using System;

namespace _StoryGame.Game.Managers.Condition
{
    [Serializable]
    public struct ConditionData
    {
        /// <summary>
        ///  Подразумевает что невозможно взаимодействие с объектом (эл щиток затоплен, нужно выключить воду - уйет лужа - станет доступен)
        /// </summary>
        public BlockingCondition[] blockingConditions;

        /// <summary>
        /// Глобальные кондишены, но выдающие мысль
        /// </summary>
        public InteractCondition[] conditions;

        /// <summary>
        /// Необходимые предметы (может быть сразу несколько) (должны быть все в инвентаре)
        /// </summary>
        public ItemCondition[] requiredItems;

        /// <summary>
        ///  Один из предметов подходяжих для выполнения условия (сломать ящик - лом или молоток)
        /// </summary>
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
}
