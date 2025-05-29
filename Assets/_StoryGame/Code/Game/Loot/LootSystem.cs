using System;
using System.Collections.Generic;
using _StoryGame.Data.Interactable;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Game.Interactables.Interfaces;
using _StoryGame.Game.Loot.Interfaces;
using _StoryGame.Game.Room;
using _StoryGame.Infrastructure.Logging;
using UnityEngine;

namespace _StoryGame.Game.Loot
{
    // в каждой комнате лут будет гененрироваться, при смене комнаты - сбрасываться

    /// <summary>
    /// Гененрирует лут для комнат и держит кэш сгенерированного лута (в виде типов)
    /// Будет хранить лут для всех комнат, для каких-либо взаимосвязей
    /// </summary>
    public sealed class LootSystem : ILootSystem
    {
        private readonly IJLog _log;

        // <roomId, room loot types>
        private readonly Dictionary<string, GeneratedRoomLootTypes> _roomsLootCache = new();

        // <(roomId, inspectableId), concrete loot>
        private readonly Dictionary<(string, string), GeneratedLootForInspectableVo> _inspectableLootCache = new();

        private readonly LootGenerator _lootGenerator;

        public LootSystem(IJLog log)
        {
            _log = log;
            _lootGenerator = new LootGenerator();
            // subscribe to change room evt - reset loot
        }

        /// <summary>
        /// Генерирует лут для комнаты
        /// </summary>
        public bool GenerateLoot(IRoom room)
        {
            Debug.Log("Generate Loot for Room: " + room.Id);

            var lootTypes = _lootGenerator.GenerateLoot(room.Id, room.Interactables.GetWrappedInspectables());

            _roomsLootCache.TryAdd(room.Id, lootTypes);

            var inspectableLootData = room.GetInspectableLootData();

            foreach (var interactableLoot in lootTypes.Loot)
            {
                var concreteLoot = GetLootByType(inspectableLootData, interactableLoot.Value);

                _inspectableLootCache.TryAdd((room.Id, interactableLoot.Key), concreteLoot);
            }

            return true;
        }

        private GeneratedLootForInspectableVo GetLootByType(InspectableLootVo inspectableLootData,
            List<LootType> lootTypes)
        {
            var result = new List<ACurrencyData>();

            foreach (var lootType in lootTypes)
            {
                switch (lootType)
                {
                    case LootType.Core:
                        result.Add(inspectableLootData.coreItem.coreItemData);
                        break;
                    case LootType.Note:
                        result.Add(inspectableLootData.notes.notes[0]); // TODO думать
                        break;
                    case LootType.Energy:
                        result.Add(inspectableLootData.energy.energy);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return new GeneratedLootForInspectableVo(result);
        }

        public GeneratedLootForInspectableVo GetGeneratedLoot(string roomId, string inspectableId)
        {
            if (_inspectableLootCache.ContainsKey((roomId, inspectableId)))
                return _inspectableLootCache[(roomId, inspectableId)];

            throw new KeyNotFoundException($"Loot for interactable {inspectableId} in room {roomId} not found!");
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

    /// <summary>
    /// Содержит список типов лута для какждого исследуемого объекта по Id
    /// </summary>
    public record GeneratedRoomLootTypes(Dictionary<string, List<LootType>> Loot)
    {
        public Dictionary<string, List<LootType>> Loot { get; } = Loot; // <inspectable id, loot types>
    }

    /// <summary>
    /// Содержит список конкретного лута
    /// </summary>
    public record GeneratedLootForInspectableVo(List<ACurrencyData> Loot)
    {
        public List<ACurrencyData> Loot { get; } = Loot;
    }
}
