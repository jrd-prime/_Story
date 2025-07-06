using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Systems;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Interactables
{
    /// <summary>
    /// Объекты с несколькими стадиями взаимодействия
    /// </summary>
    public sealed class MultiStage : AInteractable<InspectSystem>// TODO fake system
    {
        public override EInteractableType InteractableType => EInteractableType.MultiStage;
        public override UniTask InteractAsync(ICharacter character)
        {
            throw new NotImplementedException();
        }

    }
}
