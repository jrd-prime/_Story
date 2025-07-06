using _StoryGame.Core.Common.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace _StoryGame.Core.Providers.Assets
{
    public interface IAssetProvider : IBootable
    {
        UniTask<SceneInstance> LoadSceneAsync(string assetId, LoadSceneMode loadSceneMode);
        UniTask<GameObject> InstantiateAsync(AssetReference assetId, Transform parent = null);
        GameObject Instantiate(AssetReferenceGameObject assetId, Transform parent = null);
        T LoadAsset<T>(string id);
    }
}
