using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Room;

namespace _StoryGame.Game.Managers.Game.Messages
{
    public record GoToRoomRequestMsg(EExit ToExit,ERoom FromRoom, ERoom ToRoom) : IGameManagerMsg;
}
