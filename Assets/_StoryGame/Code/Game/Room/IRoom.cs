using System;
using System.Collections;
using System.Collections.Generic;
using _StoryGame.Data.Room;
using _StoryGame.Game.Loot;

namespace _StoryGame.Game.Room
{
    public interface IRoom
    {
        string Id { get; }
        string Name { get; }
        float Progress { get; }
        RoomLootVo Loot { get; }
        RoomInteractablesVo Interactables { get; }
        List<LootType> GetLootFor(string id);
    }
}
