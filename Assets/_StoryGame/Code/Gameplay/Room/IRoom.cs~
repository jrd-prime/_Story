using System;
using System.Collections.Generic;

namespace _StoryGame.Gameplay.Room
{
    public interface IRoom
    {
        string Name { get; } // Название комнаты
        float Progress { get; } // Прогресс исследования (0–100%)
        List<Loot> LootPool { get; } // Список возможного лута
        List<Transition> Transitions { get; } // Переходы в другие комнаты
        void Inspect(); // Осмотр (+10% прогресса, шанс лута)
        void DeepSearch(); // Углубленный поиск (-1 энергия, 40% шанс)
        void UnlockObject(); // Вскрытие объекта (-2–4 энергии)
        bool CanTransition(Transition transition); // Проверка перехода
    }

    [Serializable]
    public record Transition(string Name, int EnergyCost)
    {
        public string Name { get; } = Name;
        public int EnergyCost { get; } = EnergyCost;
    }

    [Serializable]
    public record Loot(string Name, int EnergyCost)
    {
        public string Name { get; } = Name;
        public int EnergyCost { get; } = EnergyCost;
    }
}
