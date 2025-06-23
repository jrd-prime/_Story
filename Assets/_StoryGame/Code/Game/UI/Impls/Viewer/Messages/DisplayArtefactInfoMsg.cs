using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data.Loot;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record DisplayArtefactInfoMsg(
        LootData ConditionalLoot,
        UniTaskCompletionSource<EDialogResult> CompletionSource) : IUIViewerMsg;
}
