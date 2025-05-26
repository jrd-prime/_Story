using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Game.Interactables.Data;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interactables.Types
{
    /// <summary>
    /// Объект, который может быть использован, в т.ч. залутан(т.е. взят в рюкзак). Дверь, выключатель, предмет на полу
    /// </summary>
    public class Usable : AInteractable
    {
        public override EInteractableType InteractableType => EInteractableType.Use;

        public override UniTask InteractAsync(ICharacter character)
        {
            return UniTask.CompletedTask;
        }
    }
}
