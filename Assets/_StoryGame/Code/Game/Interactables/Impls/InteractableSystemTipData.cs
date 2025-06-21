using System;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _StoryGame.Game.Interactables.Impls
{
    [CreateAssetMenu(fileName = nameof(InteractableSystemTipData),
        menuName = SOPathConst.Settings + nameof(InteractableSystemTipData))]
    public sealed class InteractableSystemTipData : ASettingsBase
    {
        [SerializeField] private InteractableSystemTipVo hasLoot;
        [SerializeField] private InteractableSystemTipVo noLoot;
        [SerializeField] private InteractableSystemTipVo condLooted;

        public string GetRandomTip(EInteractableSystemTip eInteractableSystemTip) =>
            eInteractableSystemTip switch
            {
                EInteractableSystemTip.InspHasLoot => hasLoot.GetRandomLocalizationKey(),
                EInteractableSystemTip.InspNoLoot => noLoot.GetRandomLocalizationKey(),
                EInteractableSystemTip.CondLooted => condLooted.GetRandomLocalizationKey(),
                _ => throw new ArgumentOutOfRangeException(nameof(eInteractableSystemTip), eInteractableSystemTip,
                    null)
            };
    }

    [Serializable]
    public struct InteractableSystemTipVo
    {
        public EInteractableSystemTip type;
        public int tipCount;
        public string localizationKeyBase;

        public string GetRandomLocalizationKey()
        {
            // localizationKeyBase_01 localizationKeyBase_02 etc
            var random = Random.Range(1, tipCount + 1);
            return $"{localizationKeyBase}_{random:D2}";
        }
    }

    public enum EInteractableSystemTip
    {
        InspHasLoot,
        InspNoLoot,
        CondLooted
    }
}
