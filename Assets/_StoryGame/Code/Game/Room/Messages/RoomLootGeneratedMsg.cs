using _StoryGame.Core.Messaging.Interfaces;

namespace _StoryGame.Game.Room.Messages
{
    public record RoomLootGeneratedMsg(string RoomId) : IJMessage
    {
        public string RoomId { get; } = RoomId;
    }
}
