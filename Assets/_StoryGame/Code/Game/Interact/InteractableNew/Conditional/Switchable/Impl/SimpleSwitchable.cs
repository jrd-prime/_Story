using System;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Movement;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _StoryGame.Game.Interact.InteractableNew.Conditional.Switchable.Impl
{
    /// <summary>
    /// Объект, у которого можно переключить состояние. Выключатель, вентиль
    /// Может требовать выполнения каких-либо условий для взаимодействия.
    /// Меняет свое состояние и глобальное сценарное состояние
    /// </summary>
    public sealed class SimpleSwitchable : ASwitchable<SimpleSwitchSystem, ISimpleSwitchable>, ISimpleSwitchable
    {
        // [Title(nameof(SimpleSwitchable))]

        protected override void OnStart()
        {
            base.OnStart();
            LOG.Warn("OnStart current: " + CurrentState);
            if (ConditionChecker == null)
                throw new Exception($"ConditionChecker is null for {gameObject.name}.");

            var result = ConditionChecker.GetSwitchState(ImpactCondition);

            LOG.Warn("ImpactCondition > " + ImpactCondition + " > result: " + result + " >  current: " +
                     CurrentState);

            if (result == CurrentState)
                return;

            SetCurrentStateAsync(result).Forget();
        }
    }

    public interface ISimpleSwitchable : ISwitchable
    {
    }
}
