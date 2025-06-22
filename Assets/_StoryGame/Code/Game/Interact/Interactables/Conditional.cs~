using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interactables;
using _StoryGame.Data.Interactable;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Data.SO.Currency;
using _StoryGame.Game.Interactables.Abstract;
using _StoryGame.Game.Interactables.Impls.Systems.Cond;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interactables.Impls.ObjTypes
{
    /// <summary>
    /// Объект, для которого необходимо выполнение каких-либо условий чтобы он стал активен.
    /// После выполнения условий он "меняет" поведение
    /// </summary>
    public sealed class Conditional : AInteractable<ConditionalSystem>, IConditional
    {
        [SerializeField] private SpecialItemData loot;
        [SerializeField] private ACurrencyData[] conditionalItems;
        [SerializeField] private ThoughtData lockedStateThought;

        public SpecialItemData Loot => loot;
        public ThoughtData LockedStateThought => lockedStateThought;
        public ACurrencyData[] ConditionalItems => conditionalItems;
        public override EInteractableType InteractableType => EInteractableType.Condition;
        public EConditionalState ConditionalState { get; private set; } = EConditionalState.Unknown;

        public void SetConditionalState(EConditionalState conditionalState) =>
            ConditionalState = conditionalState;

        public override async UniTask InteractAsync(ICharacter character)
        {
            if (System == null)
                throw new Exception("ConditionalSystem is null. " + gameObject.name);

            await System.Process(this);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!loot)
                throw new Exception("Loot is null. " + name);

            if (conditionalItems == null || conditionalItems.Length == 0)
                throw new Exception("Conditional items is null or empty. " + name);

            foreach (var currencyData in conditionalItems)
            {
                if (currencyData is not CoreItemData or CoreNoteData)
                    Debug.LogWarning("Currency data is not CoreItem or CoreNote. " + name, this);
            }
        }
#endif
    }
}
