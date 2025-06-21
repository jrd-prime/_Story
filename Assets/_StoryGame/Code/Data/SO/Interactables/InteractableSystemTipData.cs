using System;
using _StoryGame.Data.Const;
using _StoryGame.Data.Interactable;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.SO.Interactables
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
}
