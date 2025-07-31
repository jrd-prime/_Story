using _StoryGame.Core.Messaging.Interfaces;

namespace _StoryGame.Game.Managers.Game.Messages
{
    public record SpendEnergyRequestMsg(int Amount) : IGameManagerMsg
    {
        public int Amount { get; } = Amount;
    }
}
