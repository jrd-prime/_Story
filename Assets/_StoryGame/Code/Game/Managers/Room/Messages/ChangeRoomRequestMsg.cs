using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Room;

namespace _StoryGame.Game.Managers.Room.Messages
{
    public record ChangeRoomRequestMsg(EExit Exit,ERoom FromRoom, ERoom ToRoom) : IRoomsDispatcherMsg;
}
