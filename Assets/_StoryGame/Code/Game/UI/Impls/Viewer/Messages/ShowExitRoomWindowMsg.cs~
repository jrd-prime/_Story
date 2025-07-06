using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Use
{
    public record ShowExitRoomWindowMsg(
        string LocalizedName,
        string Question,
        int Price,
        UniTaskCompletionSource<EDialogResult> CompletionSource) : IUIViewerMsg;
}
