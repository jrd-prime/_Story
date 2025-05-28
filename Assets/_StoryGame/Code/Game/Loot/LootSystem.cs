using System.Collections.Generic;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Game.Interactables.Interfaces;
using _StoryGame.Game.Loot.Interfaces;
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

        public GeneratedLootVo GetGeneratedLoot(IInspectable inspectable)
        {
            var roomId = inspectable.Room.Id;
            var inspectableId = inspectable.Id;

            if (_roomsLootCache.ContainsKey(roomId))
            {
                var roomLoot = _roomsLootCache[roomId];
                if (roomLoot.Loot.ContainsKey(inspectableId))
                {
                    return inspectable.Room.GetLoot(roomLoot.Loot[inspectableId]);
                }
            }

            throw new KeyNotFoundException($"Loot for interactable {inspectable} in room {roomId} not found!");
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

        public bool HasLoot(string roomId, string inspectableId)
        {
            if (_roomsLootCache.ContainsKey(roomId))
            {
                var roomLoot = _roomsLootCache[roomId];
                if (roomLoot.Loot.ContainsKey(inspectableId))
                {
                    if (roomLoot.Loot[inspectableId] == null || roomLoot.Loot[inspectableId].Count == 0)
                    {
                        _log.Info(
                            $"Loot for interactable {inspectableId} GENERATED. But it is <color=red>EMPTY</color>!");
                        return false;
                    }

                    _log.Info(
                        $"Loot for interactable {inspectableId} GENERATED. And it is <color=green>NOT EMPTY</color>!");
                    return true;
                }

                _log.Error($"Loot for interactable {inspectableId} for room {roomId} not GENERATED");
                return false;
            }

            _log.Error($"Loot for interactable {roomId} not GENERATED");
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

    public record GeneratedLootVo(List<ACurrencyData> Loot)
    {
        public List<ACurrencyData> Loot { get; } = Loot;
    }
}
