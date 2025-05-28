using System.Collections.Generic;
using System.Linq;
using _StoryGame.Game.Interactables.Impls;
using _StoryGame.Game.Room;
using UnityEngine;

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
        public void GenerateLoot(IRoom room, RoomBaseLootChanceVo chances)
        {
            Debug.LogWarning($"[LootGen] Генерация лута для комнаты: {room.Id}");

            var result = new Dictionary<Inspectable, List<LootType>>();
            var lootables = room.Interactables.inspectables;

            if (lootables == null || lootables.Count == 0)
            {
                Debug.LogError($"[LootGen] Нет доступных объектов для лута в комнате {room.Id}!");
                return;
            }

            foreach (var obj in lootables)
            {
                result[obj] = new List<LootType>();
                Debug.Log($"[LootGen] Объект доступен для лута: {obj.name} в комнате {room.Id}");
            }

            // 1. Гарантированное размещение Core
            bool coreAssigned = AssignLootGuaranteed(LootType.Core, lootables, result, room.Id);
            if (!coreAssigned)
            {
                Debug.LogError($"[LootGen] Не удалось разместить Core в комнате {room.Id}. Прерываем генерацию.");
                return; // Прерываем, если Core не размещен
            }

            // 2. Вероятностное размещение Note и Energy
            TryAssignLootWithRoomChance(LootType.Note, lootables, result, chances.noteBaseChance, room.Id);
            TryAssignLootWithRoomChance(LootType.Energy, lootables, result, chances.energyBaseChance, room.Id);

            // Логирование результатов
            Debug.LogWarning($"[LootGen] Результаты генерации для комнаты {room.Id}:");
            foreach (var kvp in result)
            {
                string lootList = kvp.Value.Count > 0 ? string.Join(", ", kvp.Value) : "Пусто";
                Debug.Log($"[LootGen] {kvp.Key.name} → {lootList}");
            }
        }

        private bool AssignLootGuaranteed(
            LootType lootType,
            List<Inspectable> lootables,
            Dictionary<Inspectable, List<LootType>> result,
            string roomId)
        {
            var weightedList = lootables
                .Select(inspectable => new
                {
                    obj = inspectable,
                    weight = inspectable.GetLootChance(lootType)
                })
                .Where(x => x.weight > 0)
                .ToList();

            if (weightedList.Count == 0)
            {
                Debug.LogError($"[LootGen] Нет объектов с положительным шансом для {lootType} в комнате {roomId}!");
                return false;
            }

            float totalWeight = weightedList.Sum(x => x.weight);
            float roll = Random.Range(0, totalWeight);
            float cumulative = 0f;

            Debug.Log($"[LootGen] Гарантированное назначение {lootType} в комнате {roomId}, Roll: {roll}, TotalWeight: {totalWeight}");

            foreach (var item in weightedList)
            {
                cumulative += item.weight;
                if (roll <= cumulative)
                {
                    result[item.obj].Add(lootType);
                    Debug.Log($"[LootGen] Назначен {lootType} объекту {item.obj.name} в комнате {roomId}");
                    return true;
                }
            }

            Debug.LogError($"[LootGen] Не удалось назначить {lootType} в комнате {roomId}, хотя объекты были!");
            return false;
        }

        private void TryAssignLootWithRoomChance(
            LootType lootType,
            List<Inspectable> lootables,
            Dictionary<Inspectable, List<LootType>> result,
            float roomChance,
            string roomId)
        {
            float globalRoll = Random.value;
            Debug.Log($"[LootGen] Проверка на выпадение {lootType} в комнате {roomId}: roll={globalRoll}, required<={roomChance}");

            if (globalRoll > roomChance)
            {
                Debug.Log($"[LootGen] {lootType} НЕ будет выдан в комнате {roomId} (по шансу)");
                return;
            }

            // Исключаем объекты, которые уже содержат Core, если требуется уникальность
            var availableLootables = lootables
                .Where(obj => !result[obj].Contains(LootType.Core)) // Уберите эту строку, если допускается размещение на объекте с Core
                .Select(inspectable => new
                {
                    obj = inspectable,
                    weight = inspectable.GetLootChance(lootType)
                })
                .Where(x => x.weight > 0)
                .ToList();

            if (availableLootables.Count == 0)
            {
                Debug.Log($"[LootGen] Нет подходящих объектов для {lootType} в комнате {roomId} (даже при успешном шансе)");
                return;
            }

            float totalWeight = availableLootables.Sum(x => x.weight);
            float objRoll = Random.Range(0, totalWeight);
            float cumulative = 0f;

            Debug.Log($"[LootGen] Попытка назначить {lootType} в комнате {roomId}, ObjRoll: {objRoll}, TotalWeight: {totalWeight}");

            foreach (var item in availableLootables)
            {
                cumulative += item.weight;
                if (objRoll <= cumulative)
                {
                    result[item.obj].Add(lootType);
                    Debug.Log($"[LootGen] Назначен {lootType} объекту {item.obj.name} в комнате {roomId}");
                    return;
                }
            }

            Debug.LogWarning($"[LootGen] Не удалось назначить {lootType} в комнате {roomId}, хотя шанс прошёл");
        }
    }
}



// using System.Collections.Generic;
// using System.Linq;
// using _StoryGame.Game.Interactables.Impls;
// using _StoryGame.Game.Room;
// using UnityEngine;
//
// namespace _StoryGame.Game.Loot
// {
//     public enum LootType
//     {
//         Core,
//         Note,
//         Energy
//     }
//
//     public sealed class LootGenerator
//     {
//         public void GenerateLoot(IRoom room, RoomBaseLootChanceVo chances)
//         {
//             Debug.LogWarning($"[LootGen] Генерация лута для комнаты: {room.Id}");
//
//             var result = new Dictionary<Inspectable, List<LootType>>();
//             var lootables = room.Interactables.inspectables;
//
//             if (lootables == null || lootables.Count == 0)
//             {
//                 Debug.LogWarning("[LootGen] Нет доступных объектов для лута!");
//                 return;
//             }
//
//             foreach (var obj in lootables)
//             {
//                 result[obj] = new List<LootType>();
//                 Debug.Log($"[LootGen] Объект доступен для лута: {obj.name}");
//             }
//
//             // 1. Гарантированный Core
//             AssignLootGuaranteed(LootType.Core, lootables, result);
//
//             // 2. Вероятностные Note и Energy
//             TryAssignLootWithRoomChance(LootType.Note, lootables, result, chances.noteBaseChance);
//             TryAssignLootWithRoomChance(LootType.Energy, lootables, result, chances.energyBaseChance);
//
//             Debug.LogWarning("[LootGen] Результаты генерации:");
//             foreach (var kvp in result)
//             {
//                 string lootList = kvp.Value.Count > 0 ? string.Join(", ", kvp.Value) : "Пусто";
//                 Debug.LogWarning($"[LootGen] {kvp.Key.name} → {lootList}");
//             }
//         }
//
//         private void TryAssignLootWithRoomChance(
//             LootType lootType,
//             List<Inspectable> lootables,
//             Dictionary<Inspectable, List<LootType>> result,
//             float roomChance)
//         {
//             float globalRoll = Random.value;
//             Debug.Log($"[LootGen] Проверка на выпадение {lootType}: roll={globalRoll}, required<={roomChance}");
//
//             // Если шанс не прошёл — не выдаём этот тип лута вовсе
//             if (globalRoll > roomChance)
//             {
//                 Debug.Log($"[LootGen] {lootType} НЕ будет выдан в этой комнате (по шансу)");
//                 return;
//             }
//
//             // Далее как обычно — ищем объект для назначения
//             var weightedList = lootables
//                 .Select(inspectable => new
//                 {
//                     obj = inspectable,
//                     weight = inspectable.GetLootChance(lootType)
//                 })
//                 .Where(x => x.weight > 0)
//                 .ToList();
//
//             if (weightedList.Count == 0)
//             {
//                 Debug.Log($"[LootGen] Нет подходящих объектов для {lootType} (даже при успешном шансе)");
//                 return;
//             }
//
//             float totalWeight = weightedList.Sum(x => x.weight);
//             float objRoll = Random.Range(0, totalWeight);
//             float cumulative = 0f;
//
//             foreach (var item in weightedList)
//             {
//                 cumulative += item.weight;
//                 if (objRoll <= cumulative)
//                 {
//                     result[item.obj].Add(lootType);
//                     Debug.Log($"[LootGen] Назначен {lootType} на {item.obj.name}");
//                     return;
//                 }
//             }
//
//             Debug.LogWarning($"[LootGen] Не удалось назначить {lootType}, хотя шанс прошёл");
//         }
//
//         private void AssignLootGuaranteed(
//             LootType lootType,
//             List<Inspectable> lootables,
//             Dictionary<Inspectable, List<LootType>> result)
//         {
//             var weightedList = lootables
//                 .Select(inspectable => new
//                 {
//                     obj = inspectable,
//                     weight = inspectable.GetLootChance(lootType)
//                 })
//                 .Where(x => x.weight > 0)
//                 .ToList();
//
//             if (weightedList.Count == 0)
//             {
//                 Debug.LogError($"[LootGen] Нет ни одного объекта с положительным шансом для {lootType}!");
//                 return;
//             }
//
//             float totalWeight = weightedList.Sum(x => x.weight);
//             float roll = Random.Range(0, totalWeight);
//             float cumulative = 0f;
//
//             Debug.Log($"[LootGen] Гарантированное назначение {lootType}, Roll: {roll}, TotalWeight: {totalWeight}");
//
//             foreach (var item in weightedList)
//             {
//                 cumulative += item.weight;
//                 if (roll <= cumulative)
//                 {
//                     result[item.obj].Add(lootType);
//                     Debug.Log($"[LootGen] Назначен {lootType} объекту {item.obj.name}");
//                     return;
//                 }
//             }
//         }
//
//         private void TryAssignLootWithRoomChance1(
//             LootType lootType,
//             List<Inspectable> lootables,
//             Dictionary<Inspectable, List<LootType>> result,
//             float roomChance) // глобальный шанс выпадения (например, 0.5 = 50%)
//         {
//             float roll = Random.value;
//             Debug.Log($"[LootGen] Шанс на выпадение {lootType} в комнате: {roomChance}, Roll: {roll}");
//
//             if (roll > roomChance)
//             {
//                 Debug.Log($"[LootGen] {lootType} не выпал по глобальному шансу");
//                 return;
//             }
//
//             // Ищем подходящие объекты
//             var weightedList = lootables
//                 .Select(inspectable => new
//                 {
//                     obj = inspectable,
//                     weight = inspectable.GetLootChance(lootType)
//                 })
//                 .Where(x => x.weight > 0)
//                 .ToList();
//
//             if (weightedList.Count == 0)
//             {
//                 Debug.Log($"[LootGen] Нет объектов с положительным шансом для {lootType}, пропускаем");
//                 return;
//             }
//
//             float totalWeight = weightedList.Sum(x => x.weight);
//             float objRoll = Random.Range(0, totalWeight);
//             float cumulative = 0f;
//
//             Debug.Log($"[LootGen] Попытка назначить {lootType}, ObjRoll: {objRoll}, TotalWeight: {totalWeight}");
//
//             foreach (var item in weightedList)
//             {
//                 cumulative += item.weight;
//                 if (objRoll <= cumulative)
//                 {
//                     result[item.obj].Add(lootType);
//                     Debug.Log($"[LootGen] Назначен {lootType} объекту {item.obj.name}");
//                     return;
//                 }
//             }
//         }
//     }
// }
