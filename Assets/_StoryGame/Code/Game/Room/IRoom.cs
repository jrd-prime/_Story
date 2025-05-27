using System;
using System.Collections.Generic;

namespace _StoryGame.Game.Room
{
    public interface IRoom
    {
        string Id { get; }
        string Name { get; }
        float Progress { get; }
    }
}
