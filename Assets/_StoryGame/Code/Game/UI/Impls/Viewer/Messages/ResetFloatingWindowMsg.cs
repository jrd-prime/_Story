using _StoryGame.Core.Interfaces.UI;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record ResetFloatingWindowMsg(string Id) : IUIViewerMsg
    {
        public string Id { get; } = Id;
    }
}
