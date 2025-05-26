using System;
using System.Collections.Generic;

namespace _StoryGame.Game.Room
{
    public interface IRoom
    {
        string Name { get; }
        float Progress { get; }
        List<Loot> LootPool { get; }
    }

    [Serializable]
    public record Transition(string Name, int EnergyCost)
    {
        public string Name { get; } = Name;
        public int EnergyCost { get; } = EnergyCost;
    }

    [Serializable]
    public record Loot(string Name, int EnergyCost)
    {
        public string Name { get; } = Name;
        public int EnergyCost { get; } = EnergyCost;
    }
}
