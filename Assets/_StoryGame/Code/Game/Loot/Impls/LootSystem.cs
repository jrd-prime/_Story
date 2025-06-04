using System.Collections.Generic;
using _StoryGame.Core.Loot;
using _StoryGame.Core.Loot.Interfaces;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Infrastructure.Assets;
using _StoryGame.Infrastructure.Logging;
using UnityEngine;

namespace _StoryGame.Game.Loot.Impls
{
    // в каждой комнате лут будет гененрироваться, при смене комнаты - сбрасываться
    public record InspectableData(string LocalizedName, List<InspectableLootDataNew> InspectablesLoot);

    public record RoomLootData(Dictionary<string, InspectableData> InspectableData)
    {
        /// <summary>
        /// InspectableId -> InspectableData  
        /// </summary>
        public Dictionary<string, InspectableData> InspectableData { get; } = InspectableData;
    }

    public record InspectableLootDataNew(string RoomId, string InspectableId, Sprite Icon, ACurrencyData Currency)
    {
        public string RoomId { get; } = RoomId;
        public string InspectableId { get; } = InspectableId;
        public Sprite Icon { get; } = Icon;
        public ACurrencyData Currency { get; } = Currency;
    }

    /// <summary>
    /// Гененрирует лут для комнат и держит кэш сгенерированного лута (в виде типов)
    /// Будет хранить лут для всех комнат, для каких-либо взаимосвязей
    /// </summary>
    public sealed class LootSystem : ILootSystem
    {
        private readonly IJLog _log;

        private readonly Dictionary<string, RoomLootData> _roomLootDataCache = new(); // <roomId, roomLootData>

        private readonly ILootGenerator _lootGenerator;

        public LootSystem(IJLog log, IAssetProvider assetProvider, ILootGenerator lootGenerator)
        {
            _log = log;
            _lootGenerator = lootGenerator;
            // subscribe to change room evt - reset loot
        }

        /// <summary>
        /// Генерирует лут для комнаты
        /// </summary>
        public bool GenerateLoot(IRoom room)
        {
            _roomLootDataCache.TryAdd(room.Id, _lootGenerator.Generate(room));
            return true;
        }


        public RoomLootData GetRoomLootData(string roomId)
        {
            if (_roomLootDataCache.TryGetValue(roomId, out var roomLoot))
                return roomLoot;

            _log.Error($"RoomLootData for room {roomId} not found.");
            return new RoomLootData(new Dictionary<string, InspectableData>());
        }

        public InspectableData GetLootForInspectable(string roomId, string inspectableId)
        {
            if (_roomLootDataCache.TryGetValue(roomId, out var roomLoot))
            {
                if (roomLoot.InspectableData.TryGetValue(inspectableId, out var inspectableData))
                    return inspectableData;

                _log.Error($"InspectableData for inspectable {inspectableId} in room {roomId} not found.");
                return new InspectableData(LocalizationConst.ErrorKey, new List<InspectableLootDataNew>());
            }

            _log.Error($"Room {roomId} not found in loot cache.");
            return new InspectableData(LocalizationConst.ErrorKey, new List<InspectableLootDataNew>());
        }


        public bool HasLoot(string roomId, string inspectableId)
        {
            if (!_roomLootDataCache.TryGetValue(roomId, out var roomLoot))
            {
                _log.Error($"Loot for room {roomId} not GENERATED");
                return false;
            }

            if (!roomLoot.InspectableData.TryGetValue(inspectableId, out var inspectableData))
            {
                _log.Error($"Loot for inspectable {inspectableId} in room {roomId} not GENERATED");
                return false;
            }

            if (inspectableData.InspectablesLoot == null || inspectableData.InspectablesLoot.Count == 0)
            {
                _log.Info($"Loot for inspectable {inspectableId} GENERATED but is <color=red>EMPTY</color>!");
                return false;
            }

            _log.Info($"Loot for inspectable {inspectableId} GENERATED and is <color=green>NOT EMPTY</color>!");
            return true;
        }


        private void ResetLoot()
        {
            _log.Debug("Reset Loot On Room Change");
            _roomLootDataCache.Clear();
        }
    }

    /// <summary>
    /// Содержит список типов лута для какждого исследуемого объекта по Id
    /// </summary>
    public record GeneratedRoomLootTypes(Dictionary<string, List<LootType>> Loot)
    {
        public Dictionary<string, List<LootType>> Loot { get; } = Loot; // <inspectable id, loot types>
    }
}
