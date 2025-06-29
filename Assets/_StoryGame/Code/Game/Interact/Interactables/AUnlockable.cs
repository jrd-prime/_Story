using System;
using System.Collections;
using System.Collections.Generic;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Systems;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.Interactables
{
    /// <summary>
    /// Объект, который требует наличия какого-либо ПРЕДМЕТА или УСЛОВИЙ для открытия (деревянный ящик - лом, дверь - ключ/питание)
    /// </summary>
    public abstract class AUnlockable : AInteractable<UnlockSystem>, IUnlockable // TODO fake system
    {
        [Title(nameof(AUnlockable))] [SerializeField]
        private UnlockConditionData unlockConditions;

        public UnlockConditionData UnlockConditions => unlockConditions;
        public override EInteractableType InteractableType => EInteractableType.Unlockable;


        public override async UniTask InteractAsync(ICharacter character)
        {
            await System.Process(this);
        }

        protected ConditionsResult CheckConditionsToUnlock()
        {
            var result = ConditionChecker.CheckConditions(unlockConditions);

            return result;
        }

        public EUnlockableState UnlockableState => EUnlockableState.NotSet;
    }

    public sealed class ConditionChecker
    {
        private readonly ConditionRegistry _conditionRegistry;
        private readonly IPlayer _player;
        private readonly IJLog _log;

        public ConditionChecker(ConditionRegistry conditionRegistry, IPlayer player, IJLog log)
        {
            _conditionRegistry = conditionRegistry;
            _player = player;
            _log = log;
        }

        public ConditionsResult CheckConditions(UnlockConditionData conditions)
        {
            List<string> thoughts = new();

            if (!conditions.HasConditions())
                return new ConditionsResult(true, thoughts.ToArray());


            var result = true;

            foreach (var condition in conditions.conditions)
            {
                result = false;
                thoughts.Add(condition.thoughtKey);
            }

            foreach (var currencyData in conditions.requiredItems)
            {
                result = false;
                thoughts.Add(currencyData.thoughtKey);
            }

            return new ConditionsResult(result, thoughts.ToArray());
        }

        public List<string> GetKeysUnfulfilledConditions(UnlockConditionData conditions)
        {
            var result = new List<string>();
            foreach (var interactCondition in conditions.conditions)
            {
                if (!_conditionRegistry.IsCompleted(interactCondition.type))
                {
                    if (interactCondition.thoughtKey == null || string.IsNullOrEmpty(interactCondition.thoughtKey))
                    {
                        _log.Error("InteractCondition has no thoughtKey");
                        continue;
                    }

                    result.Add(interactCondition.thoughtKey);

                    if (interactCondition.type == InteractConditionType.ModulePersistentClosed)
                        return result;
                }
            }


            foreach (var requiredItem in conditions.requiredItems)
            {
                if (!_player.Wallet.Has(requiredItem.currency.IconId, requiredItem.amount))
                {
                    if (requiredItem.thoughtKey == null || string.IsNullOrEmpty(requiredItem.thoughtKey))
                    {
                        _log.Error("ItemCondition has no thoughtKey");
                        continue;
                    }

                    result.Add(requiredItem.thoughtKey);
                }
            }

            return result;
        }
    }

    public class ConditionRegistry
    {
        private readonly Dictionary<InteractConditionType, bool> _conditions = new();

        public ConditionRegistry()
        {
            _conditions.Add(InteractConditionType.ModulePersistentClosed, false);
            _conditions.Add(InteractConditionType.HasElectricity, false);
        }

        public bool IsCompleted(InteractConditionType interactConditionType) =>
            _conditions[interactConditionType];
    }

    [Serializable]
    public struct UnlockConditionData
    {
        public InteractCondition[] conditions;
        public ItemCondition[] requiredItems;

        public bool HasConditions()
        {
            if (conditions is { Length: > 0 })
                return true;

            return requiredItems is { Length: > 0 };
        }
    }

    [Serializable]
    public struct ItemCondition
    {
        public string thoughtKey;
        public int amount;
        public ACurrencyData currency;
    }

    [Serializable]
    public struct InteractCondition
    {
        public string thoughtKey;
        public InteractConditionType type;
    }

    public enum InteractConditionType
    {
        NotSet = -1,
        HasElectricity,
        ModulePersistentClosed
    }

    public interface IUnlockable : IInteractable
    {
        EUnlockableState UnlockableState { get; }
    }

    public enum EUnlockableState
    {
        NotSet = -1,
        Locked,
        Unlocked,
        Looted
    }

    public record ConditionsResult(bool Success, string[] Toughts);
}
