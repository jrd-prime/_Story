using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data.Loot;

namespace _StoryGame.Game.UI.Impls.Viewer.Messages
{
    public record ShowNewNoteMsg(PreparedLootVo PreparedLoot, string Title, string Text) : IUIViewerMsg
    {
    }
}
