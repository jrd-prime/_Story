using _StoryGame.Core.Common.Interfaces;

namespace _StoryGame.Game.Buffs
{
    public interface IBuff
    {
        BuffType BuffType { get; }
        bool IsActive { get; }

        /// <summary>
        /// Длительность. // TODO надо подумать, мб типы накладывающихся дебафов сделать
        /// </summary>
        float Duration { get; }

        /// <summary>
        /// Реализация применения дебафа к цели
        /// </summary>
        void ApplyDebuff(IBuffable target);

        /// <summary>
        /// Реализация обновления дебафа(не ок название). Если тикающий, то наносит урон через определённые промежутки времени
        /// </summary>
        void UpdateDebuff(IBuffable target, float deltaTime);

        /// <summary>
        /// Реализация снятия дебафа с цели
        /// </summary>
        void RemoveDebuff(IBuffable target);
    }

    public enum BuffType
    {
        SlowSpeed,
        Poison
    }
}
