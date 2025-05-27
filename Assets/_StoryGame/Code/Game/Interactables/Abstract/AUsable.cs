using _StoryGame.Game.Interactables.Data;

namespace _StoryGame.Game.Interactables.Abstract
{
    /// <summary>
    /// Объект, который может быть использован, в т.ч. залутан(т.е. взят в рюкзак). Дверь, выключатель, предмет на полу
    /// </summary>
    public abstract class AUsable : AInteractable
    {
        public override EInteractableType InteractableType => EInteractableType.Use;
    }
}
