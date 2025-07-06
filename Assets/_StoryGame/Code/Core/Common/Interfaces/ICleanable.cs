using _StoryGame.Core.Interact.Interactables;
using R3;

namespace _StoryGame.Core.Common.Interfaces
{
    /// <summary>
    /// Интерфейс для объектов, которые можно очищать
    /// </summary>
    public interface ICleanable : IInteractable
    {
        /// <summary>
        /// Уровень загрязнения
        /// </summary>
        ReactiveProperty<float> DirtLevel { get; }
        
        /// <summary>
        /// Максимальный уровень загрязнения
        /// </summary>
        float MaxDirtLevel { get; }
        
        /// <summary>
        /// Скорость загрязнения
        /// </summary>
        float DirtAccumulationRate { get; }
        
        /// <summary>
        /// Тип загрязнения
        /// </summary>
        DirtType DirtType { get; }
        
        /// <summary>
        /// Очистить объект
        /// </summary>
        /// <param name="cleaningPower">Сила очистки</param>
        /// <returns>Эффективность очистки</returns>
        float Clean(float cleaningPower);
        
        /// <summary>
        /// Загрязнить объект
        /// </summary>
        /// <param name="amount">Количество загрязнения</param>
        void AddDirt(float amount);
        
        /// <summary>
        /// Обновить уровень загрязнения
        /// </summary>
        /// <param name="deltaTime">Прошедшее время</param>
        void UpdateDirtLevel(float deltaTime);
        
        /// <summary>
        /// Проверить, требуется ли очистка
        /// </summary>
        /// <returns>Требуется ли очистка</returns>
        bool NeedsCleaning();
    }
    
    /// <summary>
    /// Типы загрязнения
    /// </summary>
    public enum DirtType
    {
        Dust,           // Пыль
        Liquid,         // Жидкость
        Food,           // Остатки еды
        Blood,          // Кровь
        Slime,          // Слизь
        Ash,            // Пепел
        Mud,            // Грязь
        Mold,           // Плесень
        Rust,           // Ржавчина
        Ectoplasm       // Эктоплазма
    }
} 