using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.Interactables.Data;
using _StoryGame.Game.Loot;
using _StoryGame.Game.UI.Impls.Viewer.Layers;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Messages
{
    public record ShowLootWindowMsg(
        GeneratedLootForInspectableVo LootForInspectableVo,
        UniTaskCompletionSource<EInteractDialogResult> CompletionSource
    ) : IUIViewerMessage
    {
        public string Name { get; } = nameof(ShowHasLootWindowMsg);
        public GeneratedLootForInspectableVo LootForInspectableVo { get; } = LootForInspectableVo;
        public UniTaskCompletionSource<EInteractDialogResult> CompletionSource { get; } = CompletionSource;
        public FloatingWindowType WindowType => FloatingWindowType.Loot;
    }
}
