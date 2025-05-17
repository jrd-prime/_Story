using _StoryGame.Infrastructure.Bootstrap;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace _StoryGame.Infrastructure.Assets
{
    public interface IAssetProvider : IBootable
    {
        UniTask<SceneInstance> LoadSceneAsync(string assetId, LoadSceneMode loadSceneMode);
        UniTask<GameObject> InstantiateAsync(AssetReference assetId, Transform parent = null);
        GameObject Instantiate(AssetReferenceGameObject assetId, Transform parent = null);
    }

    [UsedImplicitly]
    public sealed class AssetProvider : IAssetProvider
    {
        public bool IsInitialized { get; private set; }
        public string Description => "Asset Provider";

        public async UniTask InitializeOnBoot()
        {
            await Addressables.InitializeAsync();
            IsInitialized = true;
        }

        public async UniTask<SceneInstance> LoadSceneAsync(string assetId, LoadSceneMode loadSceneMode) =>
            await Addressables.LoadSceneAsync(AssetNameConst.GameScene, loadSceneMode);

        public async UniTask<GameObject> InstantiateAsync(AssetReference assetId, Transform parent = null)
        {
            var handle = Addressables.InstantiateAsync(assetId, parent);
            return await handle.Task;
        }

        public GameObject Instantiate(AssetReferenceGameObject assetId, Transform parent = null) =>
            Addressables.InstantiateAsync(assetId, parent).Result;
    }
}
