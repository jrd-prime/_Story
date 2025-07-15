using System;
using System.Collections.Generic;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Game.Interact.InteractableNew.Conditional.Switchable.Impl;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace _StoryGame.Game.Interact.Interactables.Unlock
{
    public sealed class ConditionRegistry : IConditionRegistry, IInitializable
    {
        private readonly Dictionary<EGlobalInteractCondition, bool> _conditions = new();
        private readonly IPublisher<IConditionRegistryMsg> _selfPub;
        private readonly IJLog _log;

        public ConditionRegistry(ISubscriber<IConditionRegistryMsg> conditionRegistryMsgSub,
            IPublisher<IConditionRegistryMsg> conditionRegistryMsgPub, IJLog log)
        {
            _selfPub = conditionRegistryMsgPub;
            _log = log;
            conditionRegistryMsgSub.Subscribe(
                msg => OnSwitchGlobalConditionMsg(msg as SwitchGlobalConditionMsg),
                msg => msg is SwitchGlobalConditionMsg
            );
        }

        public void Initialize()
        {
            Debug.LogWarning("ConditionRegistry initialize > add load conditions");

            _conditions.Add(EGlobalInteractCondition.ServModuleHasPower, false);
            _conditions.Add(EGlobalInteractCondition.HasElectricity, false);
            _conditions.Add(EGlobalInteractCondition.ModulePersistentClosed, false);
            // Водоснабжение для мех. модуля включено
            _conditions.Add(EGlobalInteractCondition.MechWaterSupplySwitchedOn, true);

            if ((Enum.GetNames(typeof(EGlobalInteractCondition)).Length - 1) != _conditions.Count)
                throw new Exception("Initialized conditions count mismatch!");
        }

        private void OnSwitchGlobalConditionMsg(SwitchGlobalConditionMsg msg) =>
            SwitchGlobalConditionMsg(msg.GlobalCondition);

        private void SwitchGlobalConditionMsg(EGlobalInteractCondition type)
        {
            if (!_conditions.TryGetValue(type, out var currentValue))
                return;

            _conditions[type] = !currentValue;
            _selfPub.Publish(new GlobalConditionChangedMsg(type, _conditions[type]));
            _log.Debug("ConditionRegistry: " + type + " changed to " + _conditions[type]);
        }

        public bool IsCompleted(EGlobalInteractCondition eGlobalInteractCondition) =>
            _conditions[eGlobalInteractCondition];
    }

    public interface IConditionRegistry
    {
        bool IsCompleted(EGlobalInteractCondition eGlobalInteractCondition);
    }
}
