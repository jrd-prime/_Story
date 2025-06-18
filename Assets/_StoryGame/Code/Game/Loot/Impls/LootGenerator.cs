using System;
using System.Collections.Generic;
using System.Linq;
using _StoryGame.Core.Currency;
using _StoryGame.Core.Loot;
using _StoryGame.Core.Loot.Interfaces;
using _StoryGame.Core.Providers.Assets;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Data.Const;
using _StoryGame.Data.Interactable;
using _StoryGame.Data.Loot;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Game.Interactables.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _StoryGame.Game.Loot.Impls
{
    public sealed class LootGenerator : ILootGenerator
    {
        private string _roomId;
        private readonly IAssetProvider _assetProvider;
        private readonly ILocalizationProvider _localization;

        public LootGenerator(IAssetProvider assetProvider, ILocalizationProvider localization)
        {
            _localization = localization;
            _assetProvider = assetProvider;
        }

        public RoomLootData Generate(IRoom room)
        {
            _roomId = room.Id;
            var roomLootDataCache = new Dictionary<string, RoomLootData>();
            Debug.Log($"Generate Loot for Room: {_roomId}");

            var lootTypes = GenerateLoot(room.Interactables.GetWrappedInspectables());

            var localizationKeyMap = new Dictionary<string, string>();
            foreach (var inspectable in room.Interactables.inspectables)
                localizationKeyMap.TryAdd(inspectable.Id, inspectable.LocalizationKey);

            var inspectableLootData = room.GetLootData();
            var roomData = new Dictionary<string, InspectableData>();

            foreach (var (inspectableId, typesList) in lootTypes.Loot)
            {
                var lootList = new List<InspectableLootData>();

                foreach (var lootType in typesList)
                {
                    var loot = CreateLootData(lootType, _roomId, inspectableId, inspectableLootData);
                    if (loot != null)
                        lootList.Add(loot);
                }

                var localizationKey = localizationKeyMap.GetValueOrDefault(inspectableId, LocalizationConst.ErrorKey);
                var localizedName =
                    _localization.Localize(localizationKey, ETable.Words, ETextTransform.Upper);

                roomData[inspectableId] = new InspectableData(localizedName, lootList);
            }

            return new RoomLootData(roomData);
        }


        private InspectableLootData CreateLootData(
            ELootType type,
            string roomId,
            string inspectableId,
            InspectableLootVo inspectableLootData)
        {
            return type switch
            {
                ELootType.Core => Create(roomId, inspectableId, inspectableLootData.coreItem.coreItemData),
                ELootType.Energy => Create(roomId, inspectableId, inspectableLootData.energy.energy),
                ELootType.Note => Create(roomId, inspectableId, inspectableLootData.notes.notes[0]),
                _ => null
            };
        }

        private InspectableLootData Create(string roomId, string inspectableId, ACurrencyData data)
        {
            try
            {
                var icon = _assetProvider.LoadAsset<Sprite>("Icons/" + data.IconId + "_icon.png");
                if (icon == null)
                {
                    Debug.LogError($"Failed to load icon for ID: {data.IconId}");
                    return null;
                }

                if (data.Type is ECurrencyType.Note)
                {
                }

                return new InspectableLootData(roomId, inspectableId, icon, data);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading asset: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Генерирует не конкретный лут, а типы лута
        /// </summary>
        public GeneratedRoomLootTypesData GenerateLoot(List<IInspectable> inspectables)
        {
            var result = new Dictionary<string, List<ELootType>>();

            if (inspectables == null || inspectables.Count == 0)
                throw new Exception($"{nameof(LootGenerator)} Нет доступных объектов для лута! {_roomId}");

            foreach (var obj in inspectables)
                result[obj.Id] = new List<ELootType>();

            var coreAssigned = AssignLootGuaranteed(ELootType.Core, inspectables, result);
            if (!coreAssigned)
                throw new Exception($"{nameof(LootGenerator)} Не удалось разместить Core в комнате. {_roomId}");

            TryAssignLootWithRoomChance(ELootType.Note, inspectables, result, .8f);
            TryAssignLootWithRoomChance(ELootType.Energy, inspectables, result, .8f);

            _roomId = null;
            return new GeneratedRoomLootTypesData(result);
        }

        private bool AssignLootGuaranteed(ELootType eLootType, List<IInspectable> inspectables,
            Dictionary<string, List<ELootType>> result)
        {
            var weightedList = inspectables
                .Select(inspectable => new
                {
                    obj = inspectable,
                    weight = inspectable.GetLootChance(eLootType)
                })
                .Where(x => x.weight > 0)
                .ToList();

            if (weightedList.Count == 0)
                throw new Exception(
                    $"{nameof(LootGenerator)} Нет объектов с положительным шансом для {eLootType}! {_roomId}");


            float totalWeight = weightedList.Sum(x => x.weight);
            var roll = Random.Range(0, totalWeight);
            var cumulative = 0f;

            foreach (var item in weightedList)
            {
                cumulative += item.weight;

                if (roll > cumulative)
                    continue;

                result[item.obj.Id].Add(eLootType);
                return true;
            }

            Debug.LogError(
                $"{nameof(LootGenerator)} Не удалось назначить {eLootType} в комнате, хотя объекты были! {_roomId}");
            return false;
        }

        private void TryAssignLootWithRoomChance(
            ELootType type,
            List<IInspectable> inspectables,
            Dictionary<string, List<ELootType>> result,
            float roomChance)
        {
            var globalRoll = Random.value;

            if (globalRoll > roomChance)
            {
                Debug.Log(
                    $"<color=red>{nameof(LootGenerator)} {type} НЕ будет выдан в комнате  (по шансу) {_roomId}</color>");
                return;
            }

            var availableInspectables = inspectables
                .Where(obj =>
                    !result[obj.Id]
                        .Contains(ELootType.Core))
                .Select(inspectable => new
                {
                    obj = inspectable,
                    weight = inspectable.GetLootChance(type)
                })
                .Where(x => x.weight > 0)
                .ToList();

            if (availableInspectables.Count == 0)
            {
                Debug.Log(
                    $"{nameof(LootGenerator)} Нет подходящих объектов для {type} в комнате  (даже при успешном шансе) {_roomId}");
                return;
            }

            float totalWeight = availableInspectables.Sum(x => x.weight);
            var objRoll = Random.Range(0, totalWeight);
            var cumulative = 0f;

            foreach (var item in availableInspectables)
            {
                cumulative += item.weight;

                if (objRoll > cumulative)
                    continue;

                result[item.obj.Id].Add(type);
                return;
            }

            Debug.LogWarning(
                $"{nameof(LootGenerator)} Не удалось назначить {type} в комнате , хотя шанс прошёл {_roomId}");
        }
    }
}
