using _StoryGame.Game.Interact.todecor.Abstract;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _StoryGame.Game.Interact.todecor.Impl
{
    public sealed class Switcher : ANewInteractable, IGlobalConditionBinding
    {
        [Title(nameof(Switcher) + " settings", titleAlignment: TitleAlignments.Centered)] [SerializeField]
        private GlobalConditionEffectData globalConditionEffectVo;

        public GlobalConditionEffectData GlobalConditionEffectVo => globalConditionEffectVo;

        public override EMainInteractableType interactableType => EMainInteractableType.Switcher;
    }
}
