using System;
using VContainer;

namespace _StoryGame.Core.Extensions
{
    public static class ContainerExtensions
    {
        public static T ResolveAndCheck<T>(this IObjectResolver container, string playerFrontTriggerAreaName) where T : class
        {
            var result = container.Resolve<T>();
            if (result == null)
                throw new NullReferenceException($" {nameof(T)} is null.");
            return result;
        }
    }
}
