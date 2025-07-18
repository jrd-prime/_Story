using System;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.Switchable.Abstract;
using _StoryGame.Game.Interact.Switchable.Systems;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Switchable
{
    /// <summary>
    /// Объект, у которого можно переключить состояние. Выключатель, вентиль
    /// Может требовать выполнения каких-либо условий для взаимодействия.
    /// Меняет свое состояние и глобальное сценарное состояние
    /// </summary>
    public sealed class SimpleSwitchable : ASwitchable<SimpleSwitchSystem, ISimpleSwitchable>, ISimpleSwitchable
    {
        // [Title(nameof(SimpleSwitchable))]

        // protected override void OnStart()
        // {
        //     base.OnStart();
        //     LOG.Warn("OnStart current: " + CurrentState);
        //     if (ConditionChecker == null)
        //         throw new Exception($"ConditionChecker is null for {gameObject.name}.");
        //     
        //     var result = ConditionChecker.GetSwitchState(ImpactCondition);
        //
        //     LOG.Warn("ImpactCondition > " + ImpactCondition + " > result: " + result + " >  current: " +
        //              CurrentState);
        //
        //     if (result == CurrentState)
        //         return;
        //
        //     SetCurrentStateAsync(result).Forget();
        // }
    }
}
