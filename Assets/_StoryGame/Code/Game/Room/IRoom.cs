using System;
using System.Collections;
using System.Collections.Generic;
using _StoryGame.Data.Room;

namespace _StoryGame.Game.Room
{
    public interface IRoom
    {
        string Id { get; }
        string Name { get; }
        float Progress { get; }
        RoomLootVo Loot { get; }
        RoomInteractablesVo Interactables { get; }
    }
}
