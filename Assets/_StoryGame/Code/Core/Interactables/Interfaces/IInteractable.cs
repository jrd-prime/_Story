using _StoryGame.Core.Character.Common.Interfaces;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Core.Interactables.Interfaces
{
    public interface IInteractable
    {
        bool CanInteract { get; }
        string InteractionTipNameId { get; }
        string LocalizationKey { get; }
        UniTask InteractAsync(ICharacter character);
    }
}
