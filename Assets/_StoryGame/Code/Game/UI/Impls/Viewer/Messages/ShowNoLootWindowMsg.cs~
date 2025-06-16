using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.Interactables.Data;
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
        public FloatingWindowType WindowType => FloatingWindowType.NoLoot;
    }
}
