namespace _StoryGame.Data.SO.Abstract
{
    public abstract class ANoteData : ACurrencyData
    {
        public string GetTextLocalizationKey() => LocalizationKey + "_text";
    }
}
