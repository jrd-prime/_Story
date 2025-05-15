using _StoryGame.Infrastructure.Bootstrap;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace _StoryGame.Infrastructure
{
    public sealed class AppStarter : IInitializable
    {
        private const int PseudoDelayMs = 100;
        private const float FadeOutDurationSeconds = 1f;

        private readonly DiContainer _container;

        public AppStarter(DiContainer container) => _container = container;

        public void Initialize() => InitializeAsync().Forget();

        private async UniTask InitializeAsync()
        {
            var bootstrapLoader = _container.Resolve<BootstrapLoader>();
            var bootstrapUIController = _container.Resolve<IBootstrapUIController>();

            // Bootable services
            var firstSceneProvider = _container.Resolve<FirstSceneProvider>();

            bootstrapLoader.EnqueueBootable(firstSceneProvider);

            Debug.Log("<color=green><b>Starting Services initialization...</b></color>");
            await bootstrapLoader.StartServicesInitializationAsync(PseudoDelayMs);
            Debug.Log("<color=green><b>End Services initialization...</b></color>");

            await bootstrapUIController.FadeOutAsync(FadeOutDurationSeconds);

            var bootstrapScene = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(firstSceneProvider.FirstScene.Scene);
            await SceneManager.UnloadSceneAsync(bootstrapScene);

            Debug.LogWarning("<color=green><b>=== APP STARTED! ===</b></color>");
        }
    }
}
