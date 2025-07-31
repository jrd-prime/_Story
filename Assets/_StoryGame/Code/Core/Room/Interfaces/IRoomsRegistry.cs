namespace _StoryGame.Core.Room.Interfaces
{
    public interface IRoomsRegistry
    {
        int GetRoomsCount();
        IRoom GetRoomByType(ERoom type);
    }
}
