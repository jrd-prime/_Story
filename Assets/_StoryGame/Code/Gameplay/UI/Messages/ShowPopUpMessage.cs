using _StoryGame.Gameplay.UI.Impls.Viewer;
using _StoryGame.Gameplay.UI.Impls.Viewer.Layers;
using _StoryGame.Gameplay.UI.Interfaces;
using R3;

namespace _StoryGame.Gameplay.UI.Messages
{
    public record ShowPopUpMessage(
        string Id,
        string Text,
        ReactiveCommand Command,
        PositionType PositionType = PositionType.Center)
        : IUIViewerMessage
    {
        public string Name => nameof(ShowPopUpMessage);
        public string Id { get; } = Id;
        public string Text { get; } = Text;
        public ReactiveCommand Command { get; } = Command;
        public PositionType PositionType { get; } = PositionType;
    }
}
