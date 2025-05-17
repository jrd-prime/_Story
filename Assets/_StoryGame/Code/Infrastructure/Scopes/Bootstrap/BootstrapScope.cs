using System;
using _StoryGame.Infrastructure.Bootstrap;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Infrastructure.Scopes.Bootstrap
{
    public sealed class BootstrapScope : LifetimeScope
    {
        [SerializeField] private BootstrapUIView bootstrapUIView;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"<color=cyan>{nameof(BootstrapScope)}</color>");

            if (!bootstrapUIView)
                throw new NullReferenceException("BootstrapUIView is null. " + nameof(BootstrapScope));

            BindBootstrap(builder);

            builder.RegisterEntryPoint<AppStarter>().As<IInitializable>();
        }

        private void BindBootstrap(IContainerBuilder builder)
        {
            var view = Instantiate(bootstrapUIView);

            builder.RegisterComponent(view);


            builder.Register<BootstrapUIController>(Lifetime.Singleton).AsImplementedInterfaces();
            // builder.RegisterComponentInNewPrefab(bootstrapUIView, Lifetime.Singleton);
            builder.Register<BootstrapLoader>(Lifetime.Singleton);
            Debug.LogWarning(" <color=green><b>=== BOOTSTRAP SCOPE INITIALIZED! ===</b></color>");
        }
    }
}
