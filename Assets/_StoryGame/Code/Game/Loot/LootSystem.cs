using System.Collections.Generic;
using _StoryGame.Game.Interactables;
using _StoryGame.Infrastructure.Logging;

namespace _StoryGame.Game.Loot
{
    // в каждой комнате лут будет гененрироваться, при смене комнаты - сбрасываться
    public sealed class LootSystem : ILootSystem
    {
        private readonly IJLog _log;
        private readonly Dictionary<string, bool> _hasLootCache = new(); // <interactable id, hasLoot>

        private readonly Dictionary<string, GeneratedLootData>
            _generatedLootCache = new(); // <interactable id, loot data>

        public LootSystem(IJLog log)
        {
            _log = log;
            // subscribe to change room evt - reset loot
        }

        public GeneratedLootData GetGeneratedLoot(string id)
        {
            if (!_hasLootCache.TryGetValue(id, out var value))
                throw new KeyNotFoundException($"Loot for interactable {id} not GENERATED! Generate it first!");

            if (!value)
                _log.Warn($"Loot for interactable {id} is EMPTY");

            if (_generatedLootCache.TryGetValue(id, out var lootData))
                return lootData;

            throw new KeyNotFoundException($"Loot for interactable {id} not found!");
        }

        public void GenerateLootFor(Interactable interactable)
        {
            if (_hasLootCache.ContainsKey(interactable.Id))
                return;

            // generate
            // add loot and state
            if (true)
            {
                _log.Warn(
                    $"Loot for interactable {interactable.Id} GENERATED. And it is <color=green>NOT EMPTY</color>!");
                _hasLootCache.Add(interactable.Id, true);
                _generatedLootCache.Add(interactable.Id, new GeneratedLootData());
            }
            else
            {
                _log.Warn($"Loot for interactable {interactable.Id} GENERATED. But it is <color=red>EMPTY</color>!");
                _hasLootCache.Add(interactable.Id, false);
            }
        }

        public bool HasLoot(string id)
        {
            if (_hasLootCache.TryGetValue(id, out var hasLoot))
                return hasLoot;

            _log.Error($"Loot for interactable {id} not GENERATED");
            return false;
        }

        private void ResetLoot()
        {
            _log.Debug("Reset Loot On Room Change");
            _hasLootCache.Clear();
            _generatedLootCache.Clear();
        }
    }

    public interface ILootSystem
    {
        GeneratedLootData GetGeneratedLoot(string id);
        void GenerateLootFor(Interactable interactable);
        bool HasLoot(string id);
    }

    public struct GeneratedLootData
    {
    }
}
