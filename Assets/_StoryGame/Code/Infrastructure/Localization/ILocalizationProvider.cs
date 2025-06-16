using _StoryGame.Infrastructure.Bootstrap.Interfaces;

namespace _StoryGame.Infrastructure.Localization
{
    public interface ILocalizationProvider : IBootable
    {
        string Localize(string key, ETable tableType, ETextTransform transform = ETextTransform.None);
    }
}
