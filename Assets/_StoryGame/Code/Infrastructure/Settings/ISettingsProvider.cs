using _StoryGame.Data;
using _StoryGame.Infrastructure.Bootstrap;

namespace _StoryGame.Infrastructure.Settings
{
    public interface ISettingsProvider : IBootable
    {
        T GetSettings<T>() where T : SettingsBase;
    }
}
