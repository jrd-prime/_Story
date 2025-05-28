using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.Interactables.Data;
using _StoryGame.Game.Loot;
using _StoryGame.Game.UI.Impls.Viewer.Layers;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Messages
{
    public record ShowLootWindowMsg(
        GeneratedLootVo LootVo,
        UniTaskCompletionSource<EInteractDialogResult> CompletionSource
    ) : IUIViewerMessage
    {
        public string Name { get; } = nameof(ShowHasLootWindowMsg);
        public GeneratedLootVo LootVo { get; } = LootVo;
        public UniTaskCompletionSource<EInteractDialogResult> CompletionSource { get; } = CompletionSource;
        public FloatingWindowType WindowType => FloatingWindowType.Loot;
    }
}
