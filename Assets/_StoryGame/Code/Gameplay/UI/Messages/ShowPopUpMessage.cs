using R3;
using UnityEngine.UIElements;

namespace _StoryGame.Gameplay.UI.Impls
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
