using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Game.Loot.Impls;

namespace _StoryGame.Core.Loot.Interfaces
{
    public interface ILootGenerator
    {
        RoomLootData Generate(IRoom room);
    }
}
