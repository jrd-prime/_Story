using System.Collections.Generic;
using System.Linq;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Managers;

namespace _StoryGame.Game.Managers.Condition
{
    public sealed class ConditionChecker
    {
        private readonly IConditionRegistry _conditionRegistry;
        private readonly IPlayer _player;
        private readonly IJLog _log;

        public ConditionChecker(IConditionRegistry conditionRegistry, IPlayer player, IJLog log)
        {
            _conditionRegistry = conditionRegistry;
            _player = player;
            _log = log;
        }

        public ConditionsResult CheckConditions(ConditionData conditions)
        {
            List<string> thoughts = new();

            if (!conditions.HasConditions())
                return new ConditionsResult(true, thoughts.ToArray());


            var result = true;

            foreach (var condition in conditions.conditions)
            {
                if (_conditionRegistry.IsCompleted(condition.type))
                    continue;

                thoughts.Add(condition.thoughtKey);
                result = false;
            }

            if (!result)
                return new ConditionsResult(false, thoughts.ToArray());

            foreach (var item in conditions.oneOfItem.items)
            {
                if (_player.Wallet.Has(item.currency.Id, item.amount))
                {
                    thoughts.Add(item.thoughtKey);
                    return new ConditionsResult(true, thoughts.ToArray());
                }

                result = false;
            }

            if (!result)
            {
                _log.Warn("return false after one of item");
                thoughts.Add(conditions.oneOfItem.thoughtKey);
                return new ConditionsResult(false, thoughts.ToArray());
            }

            foreach (var currencyData in conditions.requiredItems)
            {
                result = false;
                thoughts.Add(currencyData.thoughtKey);
            }


            _log.Warn("return true after required items");
            return new ConditionsResult(result, thoughts.ToArray());
        }

        public ConditionsResult GetKeysUnfulfilledConditions(ConditionData conditions)
        {
            var thoughts = new List<string>();
            bool isFulfilled = true;

            // Уровень 1: Проверка последовательных условий (самое важное)
            var sortedConditions = conditions.conditions.OrderBy(c => c.queueIndex).ToArray();
            _log.Warn("checking conditions");
            _log.Warn("Interact Conditions Count: " + sortedConditions.Length);
            foreach (var condition in sortedConditions)
            {
                if (_conditionRegistry.IsCompleted(condition.type))
                    continue;

                thoughts.Add(condition.thoughtKey);
                _log.Warn($"condition not fulfilled: {condition.thoughtKey}");
                return new ConditionsResult(false, thoughts.ToArray());
            }

            // TODO можно запилить мысли типа "о, у меня есть веревка, но мб поискать лестницу. или рискнуть"
            // Уровень 2: Проверка "один из предметов"
            _log.Warn("checking one of items");
            foreach (var item in conditions.oneOfItem.items)
            {
                if (_player.Wallet.Has(item.currency.Id, item.amount))
                {
                    thoughts.Add(item.thoughtKey);
                    _log.Warn($"found one of items: {item.thoughtKey}");
                    return new ConditionsResult(true, thoughts.ToArray());
                }
            }

            // Если ни один предмет не найден, добавляем мысль об этом условии
            thoughts.Add(conditions.oneOfItem.thoughtKey);
            isFulfilled = false;
            _log.Warn($"no one of items found: {conditions.oneOfItem.thoughtKey}");

            // Если есть обязательные предметы, проверяем их
            if (conditions.requiredItems.Any())
            {
                // Уровень 3: Проверка всех обязательных предметов
                _log.Warn("checking all items");
                foreach (var item in conditions.requiredItems)
                {
                    if (!_player.Wallet.Has(item.currency.Id, item.amount))
                    {
                        thoughts.Add(item.thoughtKey);
                        isFulfilled = false;
                        _log.Warn($"required item missing: {item.thoughtKey}");
                    }
                }
            }

            return new ConditionsResult(isFulfilled, thoughts.ToArray());
        }

        public (bool, string) GetItemConditionThought(OneOfItemCondition oneOfItem)
        {
            foreach (var item in oneOfItem.items)
                if (_player.Wallet.Has(item.currency.Id, item.amount))
                    return (true, item.thoughtKey);

            return (false, string.Empty);
        }

        public ESwitchState GetSwitchState(EGlobalCondition impactOnCondition)
        {
            if (_conditionRegistry.IsCompleted(impactOnCondition))
                return ESwitchState.On;
            return ESwitchState.Off;
        }

        public bool GetConditionState(EGlobalCondition condition) =>
            _conditionRegistry.IsCompleted(condition);

        public bool IsBlockedToInteract(BlockingCondition[] blockingConditions)
        {
            var result = false;
            foreach (var condition in blockingConditions)
            {
                if (_conditionRegistry.GetConditionState(condition.type) != condition.value)
                    continue;

                return true;
            }

            return result;
        }
    }
