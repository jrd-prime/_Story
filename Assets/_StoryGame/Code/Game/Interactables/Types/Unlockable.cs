using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Game.Interactables.Data;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interactables.Types
{
    /// <summary>
    /// Например, сейф, который можно открыть
    /// </summary>
    public sealed class Unlockable : AInteractable
    {
        public override EInteractableType InteractableType => EInteractableType.Unlock;
        public override UniTask InteractAsync(ICharacter character)
        {
            throw new NotImplementedException();
        }
    }
}
