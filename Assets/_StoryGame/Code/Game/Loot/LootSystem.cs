using System.Collections.Generic;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Game.Room;
using _StoryGame.Infrastructure.Logging;
using UnityEngine;

namespace _StoryGame.Game.Loot
{
    // в каждой комнате лут будет гененрироваться, при смене комнаты - сбрасываться
    public sealed class LootSystem : ILootSystem
    {
        private readonly IJLog _log;

        private readonly Dictionary<string, RoomGeneratedLootVo> _roomsLootCache = new();
        private readonly LootGenerator _lootGenerator;

        public LootSystem(IJLog log)
        {
            _log = log;
            _lootGenerator = new LootGenerator();
            // subscribe to change room evt - reset loot
        }

        public GeneratedLootVo GetGeneratedLoot(string id)
        {
            throw new KeyNotFoundException($"Loot for interactable {id} not found!");
        }

        public bool GenerateLoot(IRoom room)
        {
            Debug.Log("Generate Loot for Room: " + room.Id);

            var loot = _lootGenerator.GenerateLoot(room.Id, room.Interactables);

            _roomsLootCache.Add(room.Id, loot);

            // loot.ShowLootDetails();

            return true;
        }

        public List<LootType> GetLootFor(string roomId, string id)
        {
            return _roomsLootCache[roomId].Loot[id];
        }

        public bool HasLoot(string id)
        {
            _log.Error($"Loot for interactable {id} not GENERATED");
            return false;
        }


        private void ResetLoot()
        {
            _log.Debug("Reset Loot On Room Change");
            _roomsLootCache.Clear();
        }
    }

    public record RoomGeneratedLootVo(Dictionary<string, List<LootType>> Loot)
    {
        public Dictionary<string, List<LootType>> Loot { get; } = Loot;

        public void ShowLootDetails()
        {
            foreach (var kvp in Loot)
            {
                var lootList = kvp.Value.Count > 0 ? string.Join(", ", kvp.Value) : "Пусто";
                Debug.LogWarning($"[LootGen] {kvp.Key} → {lootList}");
            }
        }
    }

    public record GeneratedLootVo(string OwnerId, bool HasLoot, List<ACurrencyData> Loot)
    {
        public string OwnerId { get; } = OwnerId;
        public bool HasLoot { get; } = HasLoot;
        public List<ACurrencyData> Loot { get; } = Loot;
    }
}
