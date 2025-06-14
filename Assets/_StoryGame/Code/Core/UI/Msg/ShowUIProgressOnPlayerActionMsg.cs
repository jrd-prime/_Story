using _StoryGame.Core.Interfaces.Publisher.Messages;
using _StoryGame.Game.Interactables.Data;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Core.UI.Msg
{
    // TODO название хз
    public record ShowUIProgressOnPlayerActionMsg(
        string ActionName,
        float Duration,
        UniTaskCompletionSource<EDialogResult> CompletionSource
    ) : IPlayerActionMsg
    {
        public string ActionName { get; } = ActionName;
        public float Duration { get; } = Duration;
        public UniTaskCompletionSource<EDialogResult> CompletionSource { get; } = CompletionSource;
    }
}
