using _StoryGame.Game.Interactables.Interfaces;

namespace _StoryGame.Game.Loot
{
    public interface ILootSystem
    {
        GeneratedLootVo GetGeneratedLoot(string id);
        void GenerateLootFor(IInspectable inspectable);
        bool HasLoot(string id);
    }
}
