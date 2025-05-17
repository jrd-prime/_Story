using System;
using _StoryGame.Core.Managers.Game.Impls;
using _StoryGame.Core.Managers.Game.Interfaces;
using _StoryGame.Data;
using _StoryGame.Infrastructure.Assets;
using _StoryGame.Infrastructure.Bootstrap;
using _StoryGame.Infrastructure.Localization;
using _StoryGame.Infrastructure.Logging;
using _StoryGame.Infrastructure.Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Infrastructure.Scopes
{
    public class RootScope : LifetimeScope
    {
        [SerializeField] private BootstrapSettings bootstrapSettings;
        [SerializeField] private MainSettings mainSettings;
        [SerializeField] private MobileInput input;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private JLog log;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"<color=cyan>{nameof(RootScope)}</color>");
            // SignalBusInstaller.Install(Container);

            if (!bootstrapSettings)
                throw new NullReferenceException("BootstrapSettings is null.");
            builder.RegisterInstance(bootstrapSettings);

            if (!mainSettings)
                throw new NullReferenceException("MainSettings is null.");
            builder.RegisterInstance(mainSettings);

            builder.Register<SettingsProvider>(Lifetime.Singleton).As<ISettingsProvider>();
            builder.Register<AssetProvider>(Lifetime.Singleton).As<IAssetProvider>();
            builder.Register<LocalizationProvider>(Lifetime.Singleton).As<ILocalizationProvider>();
            builder.Register<GameService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<FirstSceneProvider>(Lifetime.Singleton);

            if (!input)
                throw new NullReferenceException("input is null.");
            builder.RegisterComponent(input).As<IJInput>();

            if (!eventSystem)
                throw new NullReferenceException("eventSystem is null.");
            builder.RegisterComponent(eventSystem);

            if (!log)
                throw new NullReferenceException("log is null.");
            builder.RegisterComponentInNewPrefab(log, Lifetime.Singleton).As<IJLog>();

            builder.Register<FullScreenMovementViewModel>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }
    }
}
