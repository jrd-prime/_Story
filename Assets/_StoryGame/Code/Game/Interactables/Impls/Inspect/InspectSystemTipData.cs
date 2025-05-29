using System;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _StoryGame.Game.Interactables.Impls.Inspect
{
    [CreateAssetMenu(fileName = nameof(InspectSystemTipData),
        menuName = SOPathConst.Settings + nameof(InspectSystemTipData))]
    public sealed class InspectSystemTipData : ASettingsBase
    {
        [SerializeField] private InspectSystemTipVo hasLoot;
        [SerializeField] private InspectSystemTipVo noLoot;

        public string GetRandomTip(InspectSystemTipType inspectSystemTipType) =>
            inspectSystemTipType switch
            {
                InspectSystemTipType.HasLoot => hasLoot.GetRandomLocalizationKey(),
                InspectSystemTipType.NoLoot => noLoot.GetRandomLocalizationKey(),
                _ => throw new ArgumentOutOfRangeException(nameof(inspectSystemTipType), inspectSystemTipType, null)
            };
    }

    [Serializable]
    public struct InspectSystemTipVo
    {
        public InspectSystemTipType type;
        public int tipCount;
        public string localizationKeyBase;

        public string GetRandomLocalizationKey()
        {
            // localizationKeyBase_01 localizationKeyBase_02 etc
            var random = Random.Range(1, tipCount + 1);
            return $"{localizationKeyBase}_{random:D2}";
        }
    }

    public enum InspectSystemTipType
    {
        HasLoot,
        NoLoot
    }
}
