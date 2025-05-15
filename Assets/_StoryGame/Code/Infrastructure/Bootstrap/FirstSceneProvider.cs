using _StoryGame.Data;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
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

        public FirstSceneProvider(BootstrapSettings bootstrapSettings) => _bootstrapSettings = bootstrapSettings;

        public async UniTask InitializeOnBoot()
        {
            FirstScene = await Addressables.LoadSceneAsync(_bootstrapSettings.FirstScene, LoadSceneMode.Additive);
            IsInitialized = true;
        }
    }
}
