using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Room;
using _StoryGame.Game.Room.Abstract;

namespace _StoryGame.Game.Managers.Game.Messages
{
    public record TransitionToRoomRequestMsg(ERoom Room) : IGameManagerMsg;
}
