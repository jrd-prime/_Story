using System.Collections.Generic;

namespace _StoryGame.Core.Quest.Interfaces
{
    /// <summary>
    /// Интерфейс для управления квестами
    /// </summary>
    public interface IQuestManager
    {
        /// <summary>
        /// Список доступных квестов
        /// </summary>
        // ReactiveCollection<IQuest> AvailableQuests { get; }

        /// <summary>
        /// Список активных квестов
        /// </summary>
        // ReactiveCollection<IQuest> ActiveQuests { get; }

        /// <summary>
        /// Список завершенных квестов
        /// </summary>
        // ReactiveCollection<IQuest> CompletedQuests { get; }

        /// <summary>
        /// Добавить квест в список доступных
        /// </summary>
        /// <param name="quest">Квест</param>
        void AddAvailableQuest(IQuest quest);

        /// <summary>
        /// Удалить квест из списка доступных
        /// </summary>
        /// <param name="quest">Квест</param>
        void RemoveAvailableQuest(IQuest quest);

        /// <summary>
        /// Принять квест
        /// </summary>
        /// <param name="quest">Квест</param>
        void AcceptQuest(IQuest quest);

        /// <summary>
        /// Отменить квест
        /// </summary>
        /// <param name="quest">Квест</param>
        void CancelQuest(IQuest quest);

        /// <summary>
        /// Завершить квест
        /// </summary>
        /// <param name="quest">Квест</param>
        void CompleteQuest(IQuest quest);

        /// <summary>
        /// Провалить квест
        /// </summary>
        /// <param name="quest">Квест</param>
        void FailQuest(IQuest quest);

        /// <summary>
        /// Обновить прогресс квеста
        /// </summary>
        /// <param name="quest">Квест</param>
        /// <param name="progress">Прогресс</param>
        void UpdateQuestProgress(IQuest quest, float progress);

        /// <summary>
        /// Получить квест по идентификатору
        /// </summary>
        /// <param name="questId">Идентификатор квеста</param>
        /// <returns>Квест</returns>
        IQuest GetQuest(string questId);

        /// <summary>
        /// Получить квесты по типу
        /// </summary>
        /// <param name="questType">Тип квеста</param>
        /// <returns>Список квестов</returns>
        List<IQuest> GetQuestsByType(QuestType questType);

        /// <summary>
        /// Получить квесты по сложности
        /// </summary>
        /// <param name="difficulty">Сложность</param>
        /// <returns>Список квестов</returns>
        List<IQuest> GetQuestsByDifficulty(QuestDifficulty difficulty);

        /// <summary>
        /// Сгенерировать новые квесты
        /// </summary>
        /// <param name="count">Количество</param>
        /// <param name="minDifficulty">Минимальная сложность</param>
        /// <param name="maxDifficulty">Максимальная сложность</param>
        void GenerateNewQuests(int count, QuestDifficulty minDifficulty, QuestDifficulty maxDifficulty);
    }
}
