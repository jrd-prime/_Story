using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.Interactables.Data;
using _StoryGame.Game.Loot;
using _StoryGame.Game.UI.Impls.Viewer.Layers;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Messages
{
    public record ShowHasLootWindowMsg(
        string Tip,
        GeneratedLootForInspectableVo LootForInspectableVo,
        UniTaskCompletionSource<EInteractDialogResult> CompletionSource) : IUIViewerMessage
    {
        public string Name { get; } = nameof(ShowHasLootWindowMsg);
        public string Tip { get; } = Tip;
        public GeneratedLootForInspectableVo LootForInspectableVo { get; } = LootForInspectableVo;
        public UniTaskCompletionSource<EInteractDialogResult> CompletionSource { get; } = CompletionSource;
        public FloatingWindowType WindowType => FloatingWindowType.HasLoot;
    }
}
