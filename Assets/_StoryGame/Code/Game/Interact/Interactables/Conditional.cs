using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Data.SO.Currency;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Systems;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interact.Interactables
{
    /// <summary>
    /// Объект, для которого необходимо выполнение каких-либо УСЛОВИЙ чтобы он стал активен. (вклю)
    /// После выполнения условий он "меняет" поведение
    /// </summary>
    public sealed class Conditional : AInteractable<ConditionalSystem>, IConditional
    {
        [SerializeField] private OjectLootVo[] loot;
        [SerializeField] private ACurrencyData[] conditionalItems;
        [SerializeField] private ThoughtData lockedStateThought;

        public OjectLootVo[] Loot => loot;
        public bool HasLoot() => loot.Length > 0;

        public ThoughtData LockedStateThought => lockedStateThought;
        public ACurrencyData[] ConditionalItems => conditionalItems;
        public override EInteractableType InteractableType => EInteractableType.Condition;
        public EConditionalState ConditionalState { get; private set; } = EConditionalState.Unknown;

        public void SetConditionalState(EConditionalState conditionalState) =>
            ConditionalState = conditionalState;

        public string GetSpecialItemId()
        {
            if (loot == null || loot.Length == 0)
                throw new Exception("ObjLoot is null or empty. " + name);

            foreach (var lootVo in loot)
                if (lootVo.currency is SpecialItemData specialItemData)
                    return specialItemData.Id;

            throw new Exception("Special item id not found. " + name);
        }

        public override async UniTask InteractAsync(ICharacter character)
        {
            if (System == null)
                throw new Exception("ConditionalSystem is null. " + gameObject.name);

            await System.Process(this);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (loot == null || loot.Length == 0)
                throw new Exception("ObjLoot is null. " + name);

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
