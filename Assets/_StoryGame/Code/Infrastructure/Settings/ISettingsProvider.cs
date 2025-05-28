using _StoryGame.Data;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Data.SO.Room;
using _StoryGame.Infrastructure.Bootstrap.Interfaces;

namespace _StoryGame.Infrastructure.Settings
{
    public interface ISettingsProvider : IBootable
    {
        T GetSettings<T>() where T : ASettingsBase;
        RoomData GetRoomSettings(string roomId);
    }
}
