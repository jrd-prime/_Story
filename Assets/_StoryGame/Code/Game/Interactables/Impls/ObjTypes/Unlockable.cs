using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Data.Interactable;
using _StoryGame.Game.Interactables.Abstract;
using _StoryGame.Game.Interactables.Impls.Systems;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interactables.Impls.ObjTypes
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
