using System;
using System.Text;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Interactables.Unlock;
using _StoryGame.Game.Movement;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _StoryGame.Game.Interact.InteractableNew.Conditional.Switchable.Impl
{
    /// <summary>
    /// Объект, у которого состояние переключается динамически в зависимости от глобальных сценарных состояний
    /// Взаимодействие только на уровне "подсказки", когда объект активен. (лужа мешает проходу - подсказка при взаимодействии)
    /// Меняет свое состояние. НЕ меняет глобальное сценарное состояние
    /// Пример: лужа на полу при отключении водоснабжения - "утекает" 
    /// </summary>
    public sealed class DynamicOnConditionSwitchable : ASwitchable<DynamicOnConditionSwitchSystem, IDynamicSwitchable>,
        IDynamicSwitchable
    {
        [Title(nameof(DynamicOnConditionSwitchable))] [SerializeField]
        private bool inverseConditionImpact;

        [SerializeField] private string notFulfilledThoughtKey;

        public bool InverseConditionImpact => inverseConditionImpact;
        public string NotFulfilledThoughtKey => notFulfilledThoughtKey;

        protected override void OnStart()
        {
            if (ConditionChecker == null)
                throw new Exception($"ConditionChecker is null for {gameObject.name}.");

            var result = ConditionChecker.GetSwitchState(ImpactCondition);

            LOG.Warn("ImpactCondition > " + ImpactCondition + " > result: " + result + " >  current: " +
                     CurrentState);

            if (result == CurrentState)
                return;

            SetCurrentStateAsync(result).Forget();
        }


        protected override void OnStateChanged(ESwitchState state)
        {
            SetCollidersActive(state == ESwitchState.On);
        }

        private void SetCollidersActive(bool b)
        {
            LOG.Warn("SetCollidersActive > " + b);
            foreach (var coll in _colliders)
                coll.enabled = b;
        }
    }

    public interface IDynamicSwitchable : ISwitchable
    {
        bool InverseConditionImpact { get; }
        string NotFulfilledThoughtKey { get; }
    }

    public sealed class DynamicOnConditionSwitchSystem : AInteractSystem<IDynamicSwitchable>
    {
        public DynamicOnConditionSwitchSystem(InteractSystemDepFlyweight dep) : base(dep)
        {
        }

        protected override async UniTask<bool> OnInteractAsync()
        {
            if (Interactable == null)
                throw new Exception("Interactable is null as DynamicSwitchable.");

            var result = Dep.ConditionChecker.GetConditionState(Interactable.ImpactCondition);

            var re = Interactable.InverseConditionImpact ? !result : result;


            Dep.Log.Warn("DynamicOnConditionSwitchSystem.OnInteractAsync > result: " + result + " >  re: " + re);

            if (!re)
            {
                var a = "Line / " + Dep.L10n.Localize(Interactable.NotFulfilledThoughtKey, ETable.SmallPhrase);

                var thought = new ThoughtDataVo(a);

                Dep.Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));
            }

            return true;
        }
    }
}
