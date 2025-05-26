using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Game.Interactables.Data;
using _StoryGame.Game.Room.Impls;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interactables.Types
{
    /// <summary>
    /// Объекты с несколькими стадиями взаимодействия
    /// </summary>
    public sealed class MultiStage : AInteractable
    {
        public override EInteractableType InteractableType => EInteractableType.MultiStage;
        public override UniTask InteractAsync(ICharacter character)
        {
            throw new NotImplementedException();
        }

    }
}
