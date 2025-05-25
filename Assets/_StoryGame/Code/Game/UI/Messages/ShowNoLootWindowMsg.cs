using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.Interactables.Inspect;
using _StoryGame.Game.UI.Impls.Viewer.Layers;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Messages
{
    public record ShowNoLootWindowMsg(
        UniTaskCompletionSource<DialogResult> CompletionSource
    ) : IUIViewerMessage
    {
        public string Name { get; } = nameof(ShowHasLootWindowMsg);
        public UniTaskCompletionSource<DialogResult> CompletionSource { get; } = CompletionSource;
        public FloatingWindowType WindowType => FloatingWindowType.NoLoot;
    }
}
