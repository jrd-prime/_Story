using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Room;
using _StoryGame.Game.Room.Abstract;

namespace _StoryGame.Game.Managers.Room.Messages
{
    public record ChangeRoomRequestMsg(EExit Exit,ERoom FromRoom, ERoom ToRoom) : IRoomsDispatcherMsg;
}
