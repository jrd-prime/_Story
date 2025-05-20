using _StoryGame.Gameplay.UI.Interfaces;

namespace _StoryGame.Gameplay.UI.Messages
{
    public record ResetPopUpMessage(string Id) : IUIViewerMessage
    {
        public string Name => nameof(ResetPopUpMessage);
        public string Id { get; } = Id;
    }
}
