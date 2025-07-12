using System;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Interactables.Unlock;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.Interactables.Condition
{
    /// <summary>
    /// Вода которую можно откачать. Веревка, которая сбрасывается в люк. Т.е. есть аним
    /// </summary>
    [RequireComponent(typeof(Animation))]
    public sealed class Toggleable : AConditional<ToggleSystem>, IToggleable
    {
        [SerializeField] private EToggleableState defaultState = EToggleableState.NotSet;
        [SerializeField] private bool disableGameObjectWhenOff = true;
        [SerializeField] private AnimationClip onStateClip;
        [SerializeField] private AnimationClip offStateClip;

        private EToggleableState _currentState = EToggleableState.NotSet;
        private EConditionResult _conditionResult = EConditionResult.NotSet;
        private bool _isInitialized;
        private Animation _animation;
        private string _onClipName;
        private string _offClipName;

        private const int SpeedMul = 20;

        protected override void OnAwake()
        {
            Debug.LogError("Toggleable on awake " + gameObject.name);
            if (defaultState == EToggleableState.NotSet)
                throw new Exception(
                    $"ToggleableState not set for {name}. Please set defaultState (e.g., On for the puddle).");

            _animation = GetComponent<Animation>();

            if (!_animation)
                throw new Exception($"Animation component not found on {name}.");
            if (!onStateClip)
                throw new Exception($"ON State Animation Clip is not assigned for {name}.");
            if (!offStateClip)
                throw new Exception($"OFF State Animation Clip is not assigned for {name}.");

            _onClipName = onStateClip.name;
            _offClipName = offStateClip.name;

            if (_animation[onStateClip.name] == null)
                _animation.AddClip(onStateClip, _onClipName);

            if (_animation[offStateClip.name] == null)
                _animation.AddClip(offStateClip, _offClipName);

            AnimToOffState().Forget();

            _isInitialized = true;
        }

        private async UniTask AnimToOffState()
        {
            LOG.Warn("AnimToOffState > OFF");
            var defSpeed = _animation[_offClipName].speed;
            _animation[_offClipName].speed = 1f * SpeedMul;
            _animation.Play(_offClipName);
            _currentState = EToggleableState.Off;

            await UniTask.Delay((int)(offStateClip.length * 1000) / SpeedMul);

            _animation[_offClipName].speed = defSpeed;
        }

        protected override void Enable()
        {
            if (!_isInitialized)
                throw new Exception("Toggleable is not initialized " + name);

            ConditionChecker = Resolver.Resolve<ConditionChecker>();

            if (ConditionChecker == null)
                throw new Exception($"ConditionChecker is null for {gameObject.name}.");

            var result = ConditionChecker.CheckConditions(ConditionsData).Success;
            _conditionResult = result ? EConditionResult.Fulfilled : EConditionResult.NotFulfilled;

            ToggleState(result);
        }

        private async void ToggleState(bool result)
        {
            var targetState = result ? GetOppositeState(defaultState) : defaultState;

            if (_currentState == targetState)
            {
                LOG.Info($"Состояние уже корректно: default={defaultState}, current={_currentState}");
                return;
            }

            LOG.Warn($"Меняем состояние: default={defaultState}, current={_currentState} -> target={targetState}");
            try
            {
                await AnimStateTo(targetState);
            }
            catch (Exception ex)
            {
                LOG.Error($"Ошибка при смене состояния на {targetState}: {ex.Message}");
            }
        }

        private static EToggleableState GetOppositeState(EToggleableState state) =>
            state == EToggleableState.Off ? EToggleableState.On : EToggleableState.Off;

        private async UniTask AnimStateTo(EToggleableState off)
        {
            AnimationClip clip;
            _currentState = off;
            switch (off)
            {
                case EToggleableState.On:
                    clip = onStateClip;
                    break;
                case EToggleableState.Off:
                    clip = offStateClip;
                    break;
                case EToggleableState.NotSet:
                default:
                    throw new ArgumentOutOfRangeException(nameof(off), off, null);
            }

            var clipLength = clip.length;
            var delay = (int)(clipLength * 1000);
            _animation.Play(clip.name);

            await UniTask.Delay(delay);
        }
    }
}
