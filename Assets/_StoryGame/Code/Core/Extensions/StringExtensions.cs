namespace _StoryGame.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Capitalize(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return char.ToUpper(text[0]) + text[1..].ToLower();
        }
    }
}
