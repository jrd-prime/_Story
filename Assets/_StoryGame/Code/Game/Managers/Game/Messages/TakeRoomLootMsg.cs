using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Data.Loot;

namespace _StoryGame.Game.Managers.Game.Messages
{
    public record TakeRoomLootMsg(PreparedObjLootData ObjLoot) : IGameManagerMsg
    {
        public PreparedObjLootData ObjLoot { get; } = ObjLoot;
    }
}
