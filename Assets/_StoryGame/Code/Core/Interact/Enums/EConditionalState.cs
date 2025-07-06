namespace _StoryGame.Core.Interact.Enums
{
    public enum EConditionalState
    {
        Unknown = -1, // Неизвестное состояние - должно быть установлено при создании комнаты
        Looted, // Предмет был собран
        Locked, // Нет предмета удовлетворяющего условиям "открытия"
        Unlocked // Предмет может быть "открыт"
    }
}
