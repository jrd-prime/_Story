using System;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.todecor.Abstract;
using _StoryGame.Game.Interact.todecor.Impl;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interact.todecor.Decorators.Passive
{
    public sealed class PSyncWithConditionDecorator : ADecorator, IPassiveDecorator
    {
        public override int Priority => 100;

        protected override void InitializeInternal()
        {
        }

        protected override UniTask<EDecoratorResult> ProcessInternal(IInteractable interactable)
        {
            if (Dep.ConditionChecker == null)
                throw new Exception($"ConditionChecker is null for {interactable.Name}.");

            var conditionEffectVo = interactable.As<IGlobalConditionBinding>().GlobalConditionEffectVo;
            var state = Dep.ConditionChecker.GetConditionState(conditionEffectVo.condition);

            Debug.LogWarning($"{conditionEffectVo.condition} = {state}");

            if (conditionEffectVo.isInverse)
                state = !state;

            var newState = state ? EInteractableState.On : EInteractableState.Off;

            if (newState == interactable.CurrentState)
                return UniTask.FromResult(EDecoratorResult.Success);

            interactable.SetState(newState);
            Debug.LogWarning($"STATE CHANGED to {newState} for {interactable.Name}");
            return UniTask.FromResult(EDecoratorResult.Success);
        }
    }
}
