using System;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Game.Interactables.Impls;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _StoryGame.Data.SO.Main
{
    [CreateAssetMenu(
        fileName = nameof(MainSettings),
        menuName = SOPathConst.MainSettings + nameof(MainSettings),
        order = 0)]
    public class MainSettings : ASettingsBase
    {
        [field: SerializeField] public AssetReference FirstScene { get; private set; }
        [field: SerializeField] public BootstrapSettings BootstrapSettings { get; private set; }
        [field: SerializeField] public JLocalizationSettings LocalizationSettings { get; private set; }
        [field: SerializeField] public HeroSettings HeroSettings { get; private set; }
        [field: SerializeField] public UISettings UISettings { get; private set; }
        [field: SerializeField] public MainRoomSettings MainRoomSettings { get; private set; }
        [field: SerializeField] public InteractableSystemTipData InteractableSystemTipData { get; private set; }
        [field: SerializeField] public CurrenciesData CurrenciesData { get; private set; }


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (FirstScene == null)
                throw new Exception("FirstScene is null or invalid. " + name);

            if (!BootstrapSettings)
                throw new Exception($"{nameof(BootstrapSettings)} is null or invalid. " + name);

            if (!LocalizationSettings)
                throw new Exception($"{nameof(LocalizationSettings)} is null or invalid. " + name);

            if (!HeroSettings)
                throw new Exception($"{nameof(HeroSettings)} is null or invalid. " + name);

            if (!UISettings)
                throw new Exception($"{nameof(UISettings)} is null or invalid. " + name);

            if (!MainRoomSettings)
                throw new Exception($"{nameof(MainRoomSettings)} is null or invalid. " + name);

            if (!InteractableSystemTipData)
                throw new Exception($"{nameof(InteractableSystemTipData)} is null or invalid. " + name);

            if (!CurrenciesData)
                throw new Exception($"{nameof(CurrenciesData)} is null or invalid. " + name);
        }
#endif
    }
}
