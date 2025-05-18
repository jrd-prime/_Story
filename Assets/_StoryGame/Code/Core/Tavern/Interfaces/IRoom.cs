using System.Collections.Generic;
using R3;
using UnityEngine;

namespace _StoryGame.Core.Tavern.Interfaces
{
    /// <summary>
    /// Интерфейс для комнаты в таверне
    /// </summary>
    public interface IRoom
    {
        /// <summary>
        /// Идентификатор комнаты
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Название комнаты
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Описание комнаты
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Тип комнаты
        /// </summary>
        RoomType Type { get; }
        
        /// <summary>
        /// Уровень комнаты
        /// </summary>
        ReactiveProperty<int> Level { get; }
        
        /// <summary>
        /// Чистота комнаты
        /// </summary>
        ReactiveProperty<float> Cleanliness { get; }
        
        /// <summary>
        /// Атмосфера комнаты
        /// </summary>
        ReactiveProperty<float> Atmosphere { get; }
        
        /// <summary>
        /// Вместимость комнаты (количество клиентов)
        /// </summary>
        int Capacity { get; }
        
        /// <summary>
        /// Список мебели в комнате
        /// </summary>
        // ReactiveCollection<IFurniture> Furniture { get; }
        
        /// <summary>
        /// Список декораций в комнате
        /// </summary>
        // ReactiveCollection<IDecoration> Decorations { get; }
        
        /// <summary>
        /// Список клиентов в комнате
        /// </summary>
        // ReactiveCollection<ICustomer> Customers { get; }
        
        /// <summary>
        /// Позиция комнаты в мире
        /// </summary>
        Vector3 Position { get; }
        
        /// <summary>
        /// Размер комнаты
        /// </summary>
        Vector3 Size { get; }
        
        /// <summary>
        /// Добавить мебель в комнату
        /// </summary>
        /// <param name="furniture">Мебель</param>
        void AddFurniture(IFurniture furniture);
        
        /// <summary>
        /// Удалить мебель из комнаты
        /// </summary>
        /// <param name="furniture">Мебель</param>
        void RemoveFurniture(IFurniture furniture);
        
        /// <summary>
        /// Добавить декорацию в комнату
        /// </summary>
        /// <param name="decoration">Декорация</param>
        void AddDecoration(IDecoration decoration);
        
        /// <summary>
        /// Удалить декорацию из комнаты
        /// </summary>
        /// <param name="decoration">Декорация</param>
        void RemoveDecoration(IDecoration decoration);
        
        /// <summary>
        /// Добавить клиента в комнату
        /// </summary>
        /// <param name="customer">Клиент</param>
        /// <returns>Успешность добавления</returns>
        // bool AddCustomer(ICustomer customer);
        
        /// <summary>
        /// Удалить клиента из комнаты
        /// </summary>
        /// <param name="customer">Клиент</param>
        // void RemoveCustomer(ICustomer customer);
        
        /// <summary>
        /// Очистить комнату
        /// </summary>
        /// <param name="value">Значение очистки</param>
        void Clean(float value);
        
        /// <summary>
        /// Улучшить комнату
        /// </summary>
        void Upgrade();
        
        /// <summary>
        /// Получить бонусы комнаты
        /// </summary>
        /// <returns>Словарь бонусов</returns>
        Dictionary<BonusType, float> GetBonuses();
    }
    
    /// <summary>
    /// Типы комнат
    /// </summary>
    public enum RoomType
    {
        MainHall,        // Главный зал
        Kitchen,         // Кухня
        Bedroom,         // Спальня
        Cellar,          // Погреб
        Laboratory,      // Лаборатория
        DemonicHall,     // Демонический зал
        CryptHall,       // Криптовый зал
        AlchemyLab,      // Алхимическая лаборатория
        Storage,         // Склад
        ThroneRoom       // Тронный зал
    }
    
    /// <summary>
    /// Типы бонусов
    /// </summary>
    public enum BonusType
    {
        ReputationGain,      // Прирост репутации
        MoneyGain,           // Прирост денег
        CookingSpeed,        // Скорость приготовления
        IngredientQuality,   // Качество ингредиентов
        CustomerSatisfaction, // Удовлетворенность клиентов
        CleaningEfficiency,  // Эффективность уборки
        AtmosphereBoost      // Усиление атмосферы
    }
} 