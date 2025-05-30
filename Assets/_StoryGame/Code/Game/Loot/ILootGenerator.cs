using System.Collections.Generic;
using _StoryGame.Game.Room;

namespace _StoryGame.Game.Loot
{
    public interface ILootGenerator
    {
        bool Generate(IRoom room, in Dictionary<string, RoomLootData> roomLootDataCache);
    }
}
