using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Data.Interactable;
using _StoryGame.Data.Loot;
using _StoryGame.Data.Room;
using _StoryGame.Game.Interactables.Abstract;
using _StoryGame.Game.Interactables.Data;
using _StoryGame.Game.Interactables.Impls.Systems;
using _StoryGame.Game.Interactables.Interfaces;
using _StoryGame.Game.UI.Impls.Views.WorldViews;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interactables.Impls.Inspect
{
    /// <summary>
    /// Объект, который можно осмотреть, а если есть лут - обыскать
    /// </summary>
    public sealed class Inspectable : AInteractable<InspectSystem>, IInspectable
    {
        [SerializeField] private RoomBaseLootChanceVo lootChance;

        public override EInteractableType InteractableType => EInteractableType.Inspect;
        public EInspectState InspectState { get; private set; } = EInspectState.NotInspected;
        public RoomBaseLootChanceVo Chances => lootChance;

        public override async UniTask InteractAsync(ICharacter character) =>
            await System.Process(this);

        public void SetInspectState(EInspectState inspectState) =>
            InspectState = inspectState;

        protected override void SetAdditionalDebugInfo(InteractablesTipUI tipUI)
        {
        }

        public int GetLootChance(ELootType eLootType)
        {
            return lootChance.GetLootChance(eLootType);
        }
    }
}
