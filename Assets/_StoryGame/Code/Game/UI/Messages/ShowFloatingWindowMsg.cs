using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.UI.Impls.Viewer.Layers;
using Cysharp.Threading.Tasks;
using R3;
using Unity.VisualScripting;

namespace _StoryGame.Game.UI.Messages
{
    public record ShowFloatingWindowMsg<TResult>(
        string Id,
        string Text,
        FloatingWindowType FloatingWindowType,
        IFloatingWindowData WindowData,
        UniTaskCompletionSource<TResult> CompletionSource,
        PositionType PositionType = PositionType.Center)
        : IUIViewerMessage
    {
        public string Name => nameof(ShowFloatingWindowMsg<TResult>);
        public string Id { get; } = Id;
        public string Text { get; } = Text;
        public FloatingWindowType FloatingWindowType { get; } = FloatingWindowType;
        public IFloatingWindowData WindowData { get; } = WindowData;
        public UniTaskCompletionSource<TResult> CompletionSource { get; } = CompletionSource;
        public PositionType PositionType { get; } = PositionType;
    }

    public interface IFloatingWindowData
    {
        public string Title { get; }
    }
}
