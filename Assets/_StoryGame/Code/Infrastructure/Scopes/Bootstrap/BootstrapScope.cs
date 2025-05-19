using _StoryGame.Infrastructure.Bootstrap;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Infrastructure.Scopes.Bootstrap
{
    public sealed class BootstrapScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"<color=cyan>{nameof(BootstrapScope)}</color>");

            BindBootstrap(builder);

            builder.RegisterEntryPoint<AppStarter>().As<IInitializable>();
        }

        private void BindBootstrap(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<BootstrapUIView>();

            builder.Register<BootstrapUIController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<BootstrapLoader>(Lifetime.Singleton);
        }
    }
}
