using System;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _StoryGame.Game.Interact.InteractableNew.Conditional.Switchable
{
    public abstract class ASwitchable<TInteractSystem> : AConditional<TInteractSystem>, ISwitchable
        where TInteractSystem : AInteractSystem<ISwitchable>
    {
        [Title(nameof(ASwitchable<TInteractSystem>), titleAlignment: TitleAlignments.Centered)] [SerializeField]
        private ESwitchState defaultState = ESwitchState.NotSet;

        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip onStateClip;
        [SerializeField] private AnimationClip offStateClip;

        private const int SpeedMul = 200;

        private string _onClipName;
        private string _offClipName;
        private Collider[] _colliders;
        private bool _isInitialized;
        private ESwitchState _currentState;

        protected override void OnAwake()
        {
            if (defaultState == ESwitchState.NotSet)
                throw new NullReferenceException("Default state is not set");
            if (!_animation)
                throw new Exception($"Animation component not found on {name}.");
            if (!onStateClip)
                throw new Exception($"ON State Animation Clip is not assigned for {name}.");
            if (!offStateClip)
                throw new Exception($"OFF State Animation Clip is not assigned for {name}.");

            _onClipName = onStateClip.name;
            _offClipName = offStateClip.name;

            if (_animation[_onClipName] == null)
                _animation.AddClip(onStateClip, _onClipName);

            if (_animation[_offClipName] == null)
                _animation.AddClip(offStateClip, _offClipName);

            if (defaultState == ESwitchState.On)
                AnimToOffState().Forget();


            _colliders = gameObject.GetComponents<Collider>();
            _isInitialized = true;
        }

        private async UniTask AnimToOffState()
        {
            LOG.Warn("AnimToOffState > OFF // " + _offClipName);
            var defSpeed = _animation[_offClipName].speed;
            _animation[_offClipName].speed = 1f * SpeedMul;
            _animation.Play(_offClipName);
            _currentState = ESwitchState.Off;

            await UniTask.Delay((int)(offStateClip.length * 1000) / SpeedMul);

            _animation[_offClipName].speed = defSpeed;
        }

        protected override void Enable()
        {
        }
    }

    public interface ISwitchable : IInteractable
    {
    }
}
