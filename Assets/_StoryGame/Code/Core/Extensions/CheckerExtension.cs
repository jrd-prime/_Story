using System;

namespace _StoryGame.Core.Extensions
{
    public static class CheckerExtension
    {
        public static void CheckOnNull<T>(this T obj, string ownerName) where T : class
        {
            if (obj == null)
                throw new NullReferenceException($"{typeof(T)} is null in {ownerName}");
        }
    }
}
