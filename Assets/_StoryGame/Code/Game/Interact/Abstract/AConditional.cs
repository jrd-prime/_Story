using System;
using System.Collections.Generic;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Game.Interact.Interactables.Unlock;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Serialization;

namespace _StoryGame.Game.Interact.Abstract
{
    /// <summary>
    /// Объект, который требует наличия какого-либо ПРЕДМЕТА или УСЛОВИЙ для открытия (деревянный ящик - лом, дверь - ключ/питание)
    /// </summary>
    public abstract class AConditional<TInteractableSystem> : AInteractable<TInteractableSystem>, IUnlockable
        where TInteractableSystem : IInteractableSystem // TODO fake system
    {
        [FormerlySerializedAs("unlockConditions")] [SerializeField]
        private ConditionData conditionsData;

        public EUnlockableState UnlockableState => EUnlockableState.NotSet;
        public ConditionData ConditionsData => conditionsData;
        public override EInteractableType InteractableType => EInteractableType.Unlockable;

        protected readonly CompositeDisposable _disposables = new();

        public override async UniTask InteractAsync(ICharacter character)
        {
            await System.Process(this);
        }

        protected ConditionsResult CheckConditionsToUnlock() =>
            ConditionChecker.CheckConditions(conditionsData);

        private void OnValidate()
        {
            if (!Equals(ConditionsData, default(ConditionData)) && conditionsData.conditions.Length > 1)
            {
                var indices = new HashSet<int>();
                foreach (var condition in conditionsData.conditions)
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
}
