using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Game.UI.Impls.Viewer.Layers.Floating;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record ShowNoLootWindowMsg(
        string ObjName,
        string Tip,
        UniTaskCompletionSource<EDialogResult> CompletionSource) : IUIViewerMsg
    {
        public string ObjName { get; } = ObjName;
        public string Tip { get; } = Tip;
        public UniTaskCompletionSource<EDialogResult> CompletionSource { get; } = CompletionSource;
        public EFloatingWindowType WindowType => EFloatingWindowType.NoLoot;
    }
}
