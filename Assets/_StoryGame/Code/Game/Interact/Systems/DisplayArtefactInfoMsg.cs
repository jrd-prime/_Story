using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data;
using _StoryGame.Data.Loot;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems
{
    public record DisplayArtefactInfoMsg(
        LootData ConditionalLoot,
        UniTaskCompletionSource<EDialogResult> CompletionSource) : IUIViewerMsg;
}
