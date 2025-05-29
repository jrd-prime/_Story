using _StoryGame.Infrastructure.Bootstrap.Interfaces;

namespace _StoryGame.Infrastructure.Localization
{
    public interface ILocalizationProvider : IBootable
    {
        string LocalizeWord(string key, WordTransform wordTransform = WordTransform.None);
        string LocalizePhrase(string key, WordTransform wordTransform = WordTransform.None);
    }
}
