using System;
using System.Collections.Generic;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.WalletNew.Messages;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Systems;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _StoryGame.Game.Interact.Interactables.Unlock
{
    /// <summary>
    /// Объект, который требует наличия какого-либо ПРЕДМЕТА или УСЛОВИЙ для открытия (деревянный ящик - лом, дверь - ключ/питание)
    /// </summary>
    public abstract class AUnlockable : AInteractable<UnlockSystem>, IUnlockable // TODO fake system
    {
        [Title(nameof(AUnlockable))] [SerializeField]
        private UnlockConditionData unlockConditions;

        public EUnlockableState UnlockableState => EUnlockableState.NotSet;
        public UnlockConditionData UnlockConditions => unlockConditions;
        public override EInteractableType InteractableType => EInteractableType.Unlockable;

        protected readonly CompositeDisposable _disposables = new();

        public override async UniTask InteractAsync(ICharacter character)
        {
            await System.Process(this);
        }


        protected ConditionsResult CheckConditionsToUnlock() =>
            ConditionChecker.CheckConditions(unlockConditions);

        private void OnValidate()
        {
            if (!Equals(UnlockConditions, default(UnlockConditionData)) && unlockConditions.conditions.Length > 1)
            {
                var indices = new HashSet<int>();
                foreach (var condition in unlockConditions.conditions)
                {
                    if (!indices.Add(condition.queueIndex))
                    {
                        throw new ArgumentException(
                            $"Обнаружен дублирующийся queueIndex: {condition.queueIndex}. " +
                            "Все queueIndex должны быть уникальными.");
                    }
                }

                indices.Clear();
            }
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
        [Range(1, 100)] public int queueIndex;
        public string thoughtKey;
        public InteractConditionType type;
    }

    public record ConditionsResult(bool Success, string[] Toughts);
}
