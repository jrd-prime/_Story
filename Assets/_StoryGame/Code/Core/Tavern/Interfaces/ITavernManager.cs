using System.Collections.Generic;
using R3;

namespace _StoryGame.Core.Tavern.Interfaces
{
    /// <summary>
    /// Интерфейс для управления таверной
    /// </summary>
    public interface ITavernManager
    {
        /// <summary>
        /// Текущий уровень таверны
        /// </summary>
        ReactiveProperty<int> TavernLevel { get; }
        
        /// <summary>
        /// Репутация таверны
        /// </summary>
        ReactiveProperty<float> Reputation { get; }
        
        /// <summary>
        /// Чистота таверны
        /// </summary>
        ReactiveProperty<float> Cleanliness { get; }
        
        /// <summary>
        /// Атмосфера таверны
        /// </summary>
        ReactiveProperty<float> Atmosphere { get; }
        
        /// <summary>
        /// Список комнат в таверне
        /// </summary>
        // ReactiveCollection<IRoom> Rooms { get; }
        
        /// <summary>
        /// Список клиентов в таверне
        /// </summary>
        // ReactiveCollection<ICustomer> Customers { get; }
        
        /// <summary>
        /// Добавить комнату в таверну
        /// </summary>
        /// <param name="room">Комната</param>
        void AddRoom(IRoom room);
        
        /// <summary>
        /// Удалить комнату из таверны
        /// </summary>
        /// <param name="room">Комната</param>
        void RemoveRoom(IRoom room);
        
        /// <summary>
        /// Обновить репутацию таверны
        /// </summary>
        /// <param name="value">Значение изменения</param>
        void UpdateReputation(float value);
        
        /// <summary>
        /// Очистить таверну
        /// </summary>
        /// <param name="value">Значение очистки</param>
        void Clean(float value);
        
        /// <summary>
        /// Улучшить атмосферу таверны
        /// </summary>
        /// <param name="value">Значение улучшения</param>
        void ImproveAtmosphere(float value);
        
        /// <summary>
        /// Получить доступные улучшения для таверны
        /// </summary>
        /// <returns>Список доступных улучшений</returns>
        List<ITavernUpgrade> GetAvailableUpgrades();
        
        /// <summary>
        /// Применить улучшение к таверне
        /// </summary>
        /// <param name="upgrade">Улучшение</param>
        void ApplyUpgrade(ITavernUpgrade upgrade);
    }
} 