using System;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _StoryGame.Data.SO.Main
{
    [CreateAssetMenu(
        fileName = nameof(BootstrapSettings),
        menuName = SOPathConst.MainSettings + nameof(BootstrapSettings)
    )]
    public class BootstrapSettings : ASettingsBase
    {
        [field: SerializeField] public AssetReference FirstScene { get; private set; }

        private void OnValidate()
        {
            if (FirstScene == null) throw new Exception("FirstScene is null or invalid. " + name);
        }
    }
}
