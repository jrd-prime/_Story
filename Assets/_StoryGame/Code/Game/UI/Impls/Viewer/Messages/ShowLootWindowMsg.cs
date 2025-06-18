using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data.Loot;
using _StoryGame.Game.Interactables.Data;
using _StoryGame.Game.UI.Impls.Viewer.Layers.Floating;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record ShowLootWindowMsg(
        string ObjName,
        InspectableData InspectableData,
        UniTaskCompletionSource<EDialogResult> CompletionSource
    ) : IUIViewerMsg
    {
        public string ObjName { get; } = ObjName;
        public InspectableData InspectableData { get; } = InspectableData;
        public UniTaskCompletionSource<EDialogResult> CompletionSource { get; } = CompletionSource;
        public EFloatingWindowType WindowType => EFloatingWindowType.Loot;
    }
}
