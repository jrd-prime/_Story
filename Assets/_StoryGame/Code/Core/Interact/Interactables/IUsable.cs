using _StoryGame.Game.Interact.Abstract;

namespace _StoryGame.Core.Interact.Interactables
{
    public interface IUsable : IInteractable
    {
        EUseAction UseAction { get; }
        EUseState UseState { get; }
        void SetState(EUseState used);
    }
}
