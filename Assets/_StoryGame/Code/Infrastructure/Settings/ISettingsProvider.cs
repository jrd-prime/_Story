using _StoryGame.Data;
using _StoryGame.Data.Room;
using _StoryGame.Infrastructure.Bootstrap.Interfaces;

namespace _StoryGame.Infrastructure.Settings
{
    public interface ISettingsProvider : IBootable
    {
        T GetSettings<T>() where T : ASettingsBase;
        RoomSettings GetRoomSettings(string roomId);
    }
}
