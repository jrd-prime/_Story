using _StoryGame.Core.Interact.Interactables;

namespace _StoryGame.Game.Interact.todecor.Impl
{
    /// <summary>
    /// Предмет, который влияет на глобальное условие или зависит от него
    /// </summary>
    public interface IGlobalConditionBinding : IInteractable
    {
        GlobalConditionEffectData GlobalConditionEffectVo { get; }
    }
}
