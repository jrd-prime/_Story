using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.Interactables.Inspect;
using _StoryGame.Game.Loot;
using _StoryGame.Game.UI.Impls.Viewer.Layers;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Messages
{
    public record ShowHasLootWindowMsg(
        GeneratedLootData LootData,
        UniTaskCompletionSource<DialogResult> CompletionSource
    ) : IUIViewerMessage
    {
        public string Name { get; } = nameof(ShowHasLootWindowMsg);
        public GeneratedLootData LootData { get; } = LootData;
        public UniTaskCompletionSource<DialogResult> CompletionSource { get; } = CompletionSource;
        public FloatingWindowType WindowType => FloatingWindowType.HasLoot;
    }
}
