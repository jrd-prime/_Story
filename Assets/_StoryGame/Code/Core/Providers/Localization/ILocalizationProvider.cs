using _StoryGame.Core.Common.Interfaces;

namespace _StoryGame.Core.Providers.Localization
{
    public interface ILocalizationProvider : IBootable
    {
        string Localize(string key, ETable tableType, ETextTransform transform = ETextTransform.None);
    }
}
