using System;
using _game.Scripts.Infrastructure.Assets;
using _StoryGame.Data;
using _StoryGame.Infrastructure.Bootstrap;
using _StoryGame.Infrastructure.Localization;
using _StoryGame.Infrastructure.Settings;
using UnityEngine;
using Zenject;

namespace _StoryGame.Infrastructure.Installers
{
    public sealed class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private BootstrapSettings bootstrapSettings;
        [SerializeField] private MainSettings mainSettings;

        public override void InstallBindings()
        {
            Debug.Log("<color=cyan>ProjectInstaller</color>");
            SignalBusInstaller.Install(Container);

            if (!bootstrapSettings)
                throw new NullReferenceException("bootstrapSettings is null.");

            Container.Bind<BootstrapSettings>().FromInstance(bootstrapSettings).AsSingle().NonLazy();

            if (mainSettings == null) throw new NullReferenceException("MainSettings is null.");
            Container.Bind<MainSettings>().FromInstance(mainSettings).AsSingle().NonLazy();

            Container.Bind<ISettingsProvider>().To<SettingsProvider>().AsSingle();
            Container.Bind<IAssetProvider>().To<AssetProvider>().AsSingle();
            Container.Bind<ILocalizationProvider>().To<LocalizationProvider>().AsSingle();
            Container.Bind<FirstSceneProvider>().AsSingle();
        }
    }
}
