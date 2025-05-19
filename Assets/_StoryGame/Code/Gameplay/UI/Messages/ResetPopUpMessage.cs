using _StoryGame.Gameplay.UI.Impls;

namespace _StoryGame.Gameplay.Interactables
{
    public record ResetPopUpMessage(string Id) : IUIViewerMessage
    {
        public string Name => nameof(ResetPopUpMessage);
        public string Id { get; } = Id;
    }
}
