using System;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Currency.Interfaces;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.Providers.Settings;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Infrastructure.Bootstrap;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Infrastructure.AppStarter
{
    public sealed class AppStarter : IInitializable
    {
        private const int PseudoDelayMs = 300;
        private const float FadeOutDurationSeconds = 1f;

        private readonly IObjectResolver _container;
        private readonly IJLog _log;
        private readonly BootstrapLoader bootstrapLoader;
        private readonly FirstSceneProvider firstSceneProvider;
        private readonly IBootstrapUIController bootstrapUIController;
        private readonly AppStartHandler _appStartHandler;

        public AppStarter(IObjectResolver container)
        {
            _container = container;
            bootstrapLoader = _container.Resolve<BootstrapLoader>();
            firstSceneProvider = _container.Resolve<FirstSceneProvider>();
            bootstrapUIController = _container.Resolve<IBootstrapUIController>();
            _log = _container.Resolve<IJLog>();
            _appStartHandler = _container.Resolve<AppStartHandler>();
        }

        public void Initialize()
        {
            // QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
            InitializeAsync().Forget();
        }

        private async UniTask InitializeAsync()
        {
            // Bootable services
            var settingsProvider = _container.Resolve<ISettingsProvider>();
            var localizationProvider = _container.Resolve<IL10nProvider>();
            var currencyRegistry = _container.Resolve<ICurrencyRegistry>();

            bootstrapLoader.EnqueueBootable(settingsProvider);
            bootstrapLoader.EnqueueBootable(localizationProvider);
            bootstrapLoader.EnqueueBootable(currencyRegistry);

            _log.Info("<color=green><b>Starting Services initialization...</b></color>");

            await UniTask.WhenAll(
                bootstrapLoader.InitServicesAsync(PseudoDelayMs),
                firstSceneProvider.LoadFirstSceneAsync()
            );

            _log.Info("<color=green><b>End Services initialization...</b></color>");

            var firstScene = firstSceneProvider.FirstScene;
            if (firstScene.Scene.IsValid())
            {
                try
                {
                    await SwitchToFirstSceneAsync(firstScene);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to switch to first scene.", e);
                }
                finally
                {
                    _log.Info("<color=green><b>=== APP STARTED! ===</b></color>");
                    _appStartHandler.AppStarted();
                }
            }
        }

        private async UniTask SwitchToFirstSceneAsync(SceneInstance firstScene)
        {
            var bootstrapScene = SceneManager.GetActiveScene();

            // await bootstrapUIController.FadeOutAsync(FadeOutDurationSeconds);

            //TODO добавить исчесновение первой, и через черную на вторую

            SceneManager.SetActiveScene(firstScene.Scene);

            await SceneManager.UnloadSceneAsync(bootstrapScene);
        }
    }
}
