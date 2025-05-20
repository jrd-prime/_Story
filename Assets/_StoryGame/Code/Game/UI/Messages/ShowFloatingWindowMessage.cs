using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.UI.Impls.Viewer.Layers;
using R3;

namespace _StoryGame.Game.UI.Messages
{
    public record ShowFloatingWindowMessage(
        string Id,
        string Text,
        ReactiveCommand Command,
        PositionType PositionType = PositionType.Center)
        : IUIViewerMessage
    {
        public string Name => nameof(ShowFloatingWindowMessage);
        public string Id { get; } = Id;
        public string Text { get; } = Text;
        public ReactiveCommand Command { get; } = Command;
        public PositionType PositionType { get; } = PositionType;
    }
}
