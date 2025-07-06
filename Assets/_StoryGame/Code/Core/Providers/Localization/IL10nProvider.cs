using _StoryGame.Core.Common.Interfaces;

namespace _StoryGame.Core.Providers.Localization
{
    public interface IL10nProvider : IBootable
    {
        string Localize(string key, ETable tableType, ETextTransform transform = ETextTransform.None);
    }
}
