using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.Interactables.Data;
using _StoryGame.Game.Loot.Impls;
using _StoryGame.Game.UI.Impls.Viewer.Layers;
using _StoryGame.Game.UI.Impls.Viewer.Layers.Floating;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Messages
{
    public record ShowHasLootWindowMsg(
        string ObjName,
        string Tip,
        InspectableData InspectableData,
        UniTaskCompletionSource<EDialogResult> CompletionSource) : IUIViewerMessage
    {
        public string Name { get; } = nameof(ShowHasLootWindowMsg);
        public string ObjName { get; } = ObjName;
        public string Tip { get; } = Tip;
        public InspectableData InspectableData { get; } = InspectableData;
        public UniTaskCompletionSource<EDialogResult> CompletionSource { get; } = CompletionSource;
        public FloatingWindowType WindowType => FloatingWindowType.HasLoot;
    }
}
