using _StoryGame.Core.UI.Interfaces;

namespace _StoryGame.Game.Interact.Systems.Inspect
{
    public record CurrentOperationMsg(string CurrentOperation) : IUIViewerMsg;
}
