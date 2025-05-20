using _StoryGame.Core.Interfaces.UI;

namespace _StoryGame.Game.UI.Messages
{
    public record ResetFloatingWindowMessage(string Id) : IUIViewerMessage
    {
        public string Name => nameof(ResetFloatingWindowMessage);
        public string Id { get; } = Id;
    }
}
