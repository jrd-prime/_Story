using System;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.todecor.Abstract;
using _StoryGame.Game.Managers.Condition;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.todecor.Decorators.Passive
{
    public sealed class PSyncWithConditionDecorator : ADecorator, IPassiveDecorator
    {
        public override int Priority => 100;

        [Inject] private ConditionChecker _conditionChecker;

        public UniTask<bool> ProcessPassive(IInteractable interactable)
        {
            if (_conditionChecker == null)
                throw new Exception($"ConditionChecker is null for {interactable.Name}.");

            var conditionEffectVo = interactable.ConditionEffectVo;
            var state = _conditionChecker.GetConditionState(conditionEffectVo.condition);

            Debug.LogWarning($"{conditionEffectVo.condition} = {state}");

            if (conditionEffectVo.isInverse)
                state = !state;

            var newState = state ? EInteractableState.On : EInteractableState.Off;

            if (newState != interactable.CurrentState)
            {
                Debug.LogWarning($"PSyncWithConditionDecorator: Updating state to {newState} for {interactable.Name}");
                interactable.SetState(newState);
            }
            else
            {
                Debug.LogWarning($"PSyncWithConditionDecorator: State unchanged ({newState}) for {interactable.Name}");
            }

            return UniTask.FromResult(true);
        }
    }
}
