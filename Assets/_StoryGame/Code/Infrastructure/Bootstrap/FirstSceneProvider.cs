using System;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Data.SO.Main;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace _StoryGame.Infrastructure.Bootstrap
{
    public sealed class FirstSceneProvider : IBootable
    {
        public string Description => "Scene Provider";
        public bool IsInitialized { get; private set; }
        public SceneInstance FirstScene { get; private set; }

        private readonly BootstrapSettings _bootstrapSettings;
        private readonly IJLog _log;

        public FirstSceneProvider(BootstrapSettings bootstrapSettings, IJLog log)
        {
            _bootstrapSettings = bootstrapSettings;
            _log = log;
        }

        public async UniTask InitializeOnBoot()
        {
            if (_bootstrapSettings == null)
            {
                _log.Error("BootstrapSettings is null. Cannot load first scene. " + nameof(FirstSceneProvider));
                return;
            }

            if (_bootstrapSettings.FirstScene == null)
            {
                _log.Error("FirstScene is not set in BootstrapSettings. " + nameof(FirstSceneProvider));
                return;
            }

            try
            {
                AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(
                    _bootstrapSettings.FirstScene,
                    LoadSceneMode.Additive
                );

                await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    FirstScene = handle.Result;
                    IsInitialized = true;
                }
                else
                    _log.Error($"Failed to load scene: {_bootstrapSettings.FirstScene}. " + nameof(FirstSceneProvider));
            }
            catch (Exception ex)
            {
                _log.Error($"Exception while loading first scene: {ex}. " + nameof(FirstSceneProvider));
            }
        }

        public async UniTask<SceneInstance> LoadFirstSceneAsync()
        {
            if (_bootstrapSettings == null)
                throw new Exception("BootstrapSettings is null. Cannot load first scene. " +
                                    nameof(FirstSceneProvider));


            if (_bootstrapSettings.FirstScene == null)
                throw new Exception("FirstScene is not set in BootstrapSettings. " + nameof(FirstSceneProvider));

            try
            {
                AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(
                    _bootstrapSettings.FirstScene,
                    LoadSceneMode.Additive
                );

                await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    FirstScene = handle.Result;
                    IsInitialized = true;
                }
                else
                    _log.Error($"Failed to load scene: {_bootstrapSettings.FirstScene}. " + nameof(FirstSceneProvider));
            }
            catch (Exception ex)
            {
                _log.Error($"Exception while loading first scene: {ex}. " + nameof(FirstSceneProvider));
            }

            return FirstScene;
        }
    }
}
