using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.Interactables.Data;
using _StoryGame.Game.UI.Impls.Viewer.Layers;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Messages
{
    public record ShowNoLootWindowMsg(
        string ObjName,
        string Tip,
        UniTaskCompletionSource<EInteractDialogResult> CompletionSource) : IUIViewerMessage
    {
        public string Name { get; } = nameof(ShowHasLootWindowMsg);
        public string ObjName { get; } = ObjName;
        public string Tip { get; } = Tip;
        public UniTaskCompletionSource<EInteractDialogResult> CompletionSource { get; } = CompletionSource;
        public FloatingWindowType WindowType => FloatingWindowType.NoLoot;
    }
}
