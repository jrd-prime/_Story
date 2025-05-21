using _StoryGame.Core.Character.Common.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Core.Interactables.Interfaces
{
    public interface IInteractable
    {
        bool CanInteract { get; }
        string InteractionTipNameId { get; }
        string LocalizationKey { get; }
        string Name { get;  }
        UniTask InteractAsync(ICharacter character);
        void ShowInteractionTip((string, string) interactionTip);
        void HideInteractionTip();
        Vector3 GetEntryPoint();
    }
}
