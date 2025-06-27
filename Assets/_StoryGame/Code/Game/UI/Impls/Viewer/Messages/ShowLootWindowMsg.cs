using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data.Loot;
using _StoryGame.Game.UI.Impls.Viewer.Layers.Floating;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record ShowLootWindowMsg(
        PreparedObjLootData PreparedObjLootData,
        UniTaskCompletionSource<EDialogResult> CompletionSource
    ) : IUIViewerMsg
    {
        public PreparedObjLootData PreparedObjLootData { get; } = PreparedObjLootData;
        public UniTaskCompletionSource<EDialogResult> CompletionSource { get; } = CompletionSource;
        public EFloatingWindowType WindowType => EFloatingWindowType.Loot;
    }
}
