using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Game.Interactables.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interactables.Interfaces
{
    public interface IInteractable
    {
        string Id { get; }
        EInteractableType InteractableType { get; }
        bool CanInteract { get; set; }
        string InteractionTipNameId { get; }
        string LocalizationKey { get; }
        string Name { get; }
        UniTask InteractAsync(ICharacter character);
        void ShowInteractionTip((string, string) interactionTip);
        void HideInteractionTip();
        Vector3 GetEntryPoint();

        void SetRoom(IRoom room);
        IRoom Room { get; }
    }
}
