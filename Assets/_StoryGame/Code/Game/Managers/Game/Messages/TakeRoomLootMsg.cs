using _StoryGame.Core.Interfaces.Publisher.Messages;
using _StoryGame.Game.Loot.Impls;

namespace _StoryGame.Game.Managers.Game.Messages
{
    public record TakeRoomLootMsg(InspectableData Loot) : IGameManagerMsg
    {
        public InspectableData Loot { get; } = Loot;
    }
}
