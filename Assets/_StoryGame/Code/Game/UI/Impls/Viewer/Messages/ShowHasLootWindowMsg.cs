using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data.Loot;
using _StoryGame.Game.UI.Impls.Viewer.Layers.Floating;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record ShowHasLootWindowMsg(
        string ObjName,
        string Tip,
        PreparedObjLootData PreparedObjLootData,
        UniTaskCompletionSource<EDialogResult> CompletionSource) : IUIViewerMsg
    {
        public string ObjName { get; } = ObjName;
        public string Tip { get; } = Tip;
        public PreparedObjLootData PreparedObjLootData { get; } = PreparedObjLootData;
        public UniTaskCompletionSource<EDialogResult> CompletionSource { get; } = CompletionSource;
        public EFloatingWindowType WindowType => EFloatingWindowType.HasLoot;
    }
}
