using System;
using _StoryGame.Infrastructure.Bootstrap;
using ModestTree;
using UnityEngine;
using Zenject;

namespace _StoryGame.Infrastructure.Installers
{
    public sealed class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private BootstrapUIView bootstrapUIView;

        public override void InstallBindings()
        {
            Log.Info($"<color=cyan>{nameof(BootstrapInstaller)}</color>");

            if (!bootstrapUIView)
                throw new NullReferenceException("BootstrapUIView is null. " + nameof(BootstrapInstaller));

            BindBootstrap();

            Container.BindInterfacesTo<AppStarter>().AsSingle().NonLazy();
        }

        private void BindBootstrap()
        {
            Container.BindInterfacesTo<BootstrapUIController>().AsSingle();
            Container.Bind<BootstrapUIView>().FromComponentInNewPrefab(bootstrapUIView).AsSingle().NonLazy();
            Container.Bind<BootstrapLoader>().AsSingle().NonLazy();
        }
    }
}
