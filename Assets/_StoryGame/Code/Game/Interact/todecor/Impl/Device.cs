using _StoryGame.Game.Interact.todecor.Abstract;
using _StoryGame.Game.Interact.todecor.Decorators.Active;
using _StoryGame.Game.Interact.todecor.Impl.DeviceSystems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _StoryGame.Game.Interact.todecor.Impl
{
    [RequireComponent(typeof(ADeviceDecorator))]
    public sealed class Device : ANewInteractable, IGlobalConditionBinding
    {
        [Title(nameof(Device) + " settings", titleAlignment: TitleAlignments.Centered)]

        [SerializeField] private GlobalConditionEffectData globalConditionEffectVo;

        public GlobalConditionEffectData GlobalConditionEffectVo => globalConditionEffectVo;
        public override EMainInteractableType interactableType => EMainInteractableType.Device;

    }
}
