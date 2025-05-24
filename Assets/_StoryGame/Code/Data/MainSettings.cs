using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _StoryGame.Data
{
    [CreateAssetMenu(
        fileName = nameof(MainSettings),
        menuName = SOPathConst.MainSettings + nameof(MainSettings),
        order = 0)]
    public class MainSettings : SettingsBase
    {
        public override string ConfigName { get; } = nameof(MainSettings);
        [field: SerializeField] public AssetReference FirstScene { get; private set; }
        [field: SerializeField] public JLocalizationSettings LocalizationSettings { get; private set; }
        [field: SerializeField] public HeroSettings HeroSettings { get; private set; }
        [field: SerializeField] public UISettings UISettings { get; private set; }


        private void OnValidate()
        {
            if (FirstScene == null)
                throw new Exception("FirstScene is null or invalid. " + name);

            if (!LocalizationSettings)
                throw new Exception($"{nameof(LocalizationSettings)} is null or invalid. " + name);

            if (!HeroSettings)
                throw new Exception($"{nameof(HeroSettings)} is null or invalid. " + name);

            if (!UISettings)
                throw new Exception($"{nameof(UISettings)} is null or invalid. " + name);
        }
    }
}
