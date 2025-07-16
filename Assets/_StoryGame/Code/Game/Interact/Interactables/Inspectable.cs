using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Systems;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interact.Interactables
{
    /// <summary>
    /// Объект, который можно осмотреть, а если есть лут - обыскать
    /// </summary>
    public sealed class Inspectable : AInteractable<InspectSystem>, IInspectable
    {
        [SerializeField] private OjectLootVo[] loot;

        public override EInteractableType InteractableType => EInteractableType.Inspect;
        public EInspectState InspectState { get; private set; } = EInspectState.NotInspected;
        public OjectLootVo[] Loot => loot;
        public bool HasLoot() => loot.Length > 0;

        protected override void OnAwake()
        {
            
        }

        public override async UniTask InteractAsync(ICharacter character)
        {
            if (System == null)
                throw new Exception("InspectSystem is null. " + gameObject.name);

            await System.Process(this);
        }

        public void SetInspectState(EInspectState inspectState) =>
            InspectState = inspectState;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (loot == null || loot.Length == 0)
                throw new Exception("ObjLoot is null or empty. " + name);
        }
#endif
        protected override void Enable()
        {
            
        }
    }

    [Serializable]
    public struct OjectLootVo
    {
        public int chance;
        public ACurrencyData currency;
        public int amount;
    }
}
