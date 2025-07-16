using _StoryGame.Core.UI.Interfaces;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record CurrentOperationMsg(string CurrentOperation) : IUIViewerMsg;
}
