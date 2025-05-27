using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.Interactables.Data;
using _StoryGame.Game.Loot;
using _StoryGame.Game.UI.Impls.Viewer.Layers;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Messages
{
    public record ShowLootWindowMsg(
        GeneratedLootData LootData,
        UniTaskCompletionSource<EInteractDialogResult> CompletionSource
    ) : IUIViewerMessage
    {
        public string Name { get; } = nameof(ShowHasLootWindowMsg);
        public GeneratedLootData LootData { get; } = LootData;
        public UniTaskCompletionSource<EInteractDialogResult> CompletionSource { get; } = CompletionSource;
        public FloatingWindowType WindowType => FloatingWindowType.Loot;
    }
}
