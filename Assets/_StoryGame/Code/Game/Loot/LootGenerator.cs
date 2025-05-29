using System;
using System.Collections.Generic;
using System.Linq;
using _StoryGame.Game.Interactables.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _StoryGame.Game.Loot
{
    public enum LootType
    {
        Core,
        Note,
        Energy
    }

    public sealed class LootGenerator
    {
        private string _roomId;

        /// <summary>
        /// Генерирует не конкретный лут, а типы лута
        /// </summary>
        public GeneratedRoomLootTypes GenerateLoot(string roomId, List<IInspectable> inspectables)
        {
            _roomId = roomId;

            var result = new Dictionary<string, List<LootType>>();

            if (inspectables == null || inspectables.Count == 0)
                throw new Exception($"{nameof(LootGenerator)} Нет доступных объектов для лута! {_roomId}");

            foreach (var obj in inspectables)
                result[obj.Id] = new List<LootType>();

            var coreAssigned = AssignLootGuaranteed(LootType.Core, inspectables, result);
            if (!coreAssigned)
                throw new Exception($"{nameof(LootGenerator)} Не удалось разместить Core в комнате. {_roomId}");

            TryAssignLootWithRoomChance(LootType.Note, inspectables, result, .8f);
            TryAssignLootWithRoomChance(LootType.Energy, inspectables, result, .8f);

            _roomId = null;
            return new GeneratedRoomLootTypes(result);
        }

        private bool AssignLootGuaranteed(LootType lootType, List<IInspectable> inspectables,
            Dictionary<string, List<LootType>> result)
        {
            var weightedList = inspectables
                .Select(inspectable => new
                {
                    obj = inspectable,
                    weight = inspectable.GetLootChance(lootType)
                })
                .Where(x => x.weight > 0)
                .ToList();

            if (weightedList.Count == 0)
                throw new Exception(
                    $"{nameof(LootGenerator)} Нет объектов с положительным шансом для {lootType}! {_roomId}");


            float totalWeight = weightedList.Sum(x => x.weight);
            var roll = Random.Range(0, totalWeight);
            var cumulative = 0f;

            foreach (var item in weightedList)
            {
                cumulative += item.weight;

                if (roll > cumulative)
                    continue;

                result[item.obj.Id].Add(lootType);
                return true;
            }

            Debug.LogError(
                $"{nameof(LootGenerator)} Не удалось назначить {lootType} в комнате, хотя объекты были! {_roomId}");
            return false;
        }

        private void TryAssignLootWithRoomChance(
            LootType type,
            List<IInspectable> inspectables,
            Dictionary<string, List<LootType>> result,
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
                        .Contains(LootType.Core))
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
