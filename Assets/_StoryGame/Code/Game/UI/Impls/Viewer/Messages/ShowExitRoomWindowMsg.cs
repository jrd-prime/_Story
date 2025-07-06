using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Interfaces;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record ShowExitRoomWindowMsg(
        string LocalizedName,
        string Question,
        int Price,
        UniTaskCompletionSource<EDialogResult> CompletionSource) : IUIViewerMsg;
}
