using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interactables.Interfaces;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interactables
{
    /// <summary>
    /// Объекты с несколькими стадиями взаимодействия
    /// </summary>
    public sealed class MultiStage : Interactable
    {
        public override EInteractableType InteractableType => EInteractableType.MultiStage;
        public override UniTask InteractAsync(ICharacter character)
        {
            throw new System.NotImplementedException();
        }
    }
}
