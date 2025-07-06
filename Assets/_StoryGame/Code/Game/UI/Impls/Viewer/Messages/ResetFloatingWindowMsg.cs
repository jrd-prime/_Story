using _StoryGame.Core.UI.Interfaces;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record ResetFloatingWindowMsg(string Id) : IUIViewerMsg
    {
        public string Id { get; } = Id;
    }
}
