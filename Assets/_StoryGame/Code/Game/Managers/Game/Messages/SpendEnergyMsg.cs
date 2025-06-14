using _StoryGame.Core.Interfaces.Publisher.Messages;

namespace _StoryGame.Game.Managers.Game.Messages
{
    public record SpendEnergyMsg(int Amount) : IGameManagerMsg
    {
        public int Amount { get; } = Amount;
    }
}
