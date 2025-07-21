using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Game.Interact.todecor;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Core.Interact.Interactables
{
    public interface IInteractable
    {
        string Id { get; }
        EInteractableType InteractableType { get; }
        bool CanInteract { get; set; }
        string LocalizationKey { get; }
        string Name { get; }
        UniTask InteractAsync(ICharacter character);
        void HideInteractionTip();
        Vector3 GetEntryPoint();
        void SetRoom(IRoom room);
        IRoom Room { get; }
        int InteractEnergyCost { get; }
        EInteractableState CurrentState { get; }
        ConditionEffectData ConditionEffectVo { get; }
        void SetBlocked(bool value);
        void SetState(EInteractableState state);
        void SwitchState();
    }
}
