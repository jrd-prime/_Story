using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data.Loot;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record DisplayArtefactInfoMsg(
        PreparedLootVo PreparedItemVo,
        UniTaskCompletionSource<EDialogResult> CompletionSource) : IUIViewerMsg;
}
