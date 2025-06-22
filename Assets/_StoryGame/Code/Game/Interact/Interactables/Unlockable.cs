using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Data.Interact;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Systems.Inspect;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Interactables
{
    /// <summary>
    /// Например, сейф, который можно открыть
    /// </summary>
    public sealed class Unlockable : AInteractable<InspectSystem> // TODO fake system
    {
        public override EInteractableType InteractableType => EInteractableType.Unlock;

        public override UniTask InteractAsync(ICharacter character)
        {
            throw new NotImplementedException();
        }
    }
}
