namespace _StoryGame.Core.Common.Interfaces
{
    /// <summary>
    /// Можно одеть/взять в руки (пылесос, метла)
    /// </summary>
    public interface IEquippable
    {
        // bool IsEquipped { get; }
        void Equip();
        void Unequip();
    }
}
