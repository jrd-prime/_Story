using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Data.SO.Room;

namespace _StoryGame.Core.Providers.Settings
{
    public interface ISettingsProvider : IBootable
    {
        T GetSettings<T>() where T : ASettingsBase;
        RoomData GetRoomSettings(string roomId);
    }
}
