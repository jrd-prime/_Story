using System;
using System.Collections.Generic;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Managers;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Game.Managers.Condition.Messages;
using MessagePipe;
using VContainer.Unity;

namespace _StoryGame.Game.Managers.Condition
{
    public sealed class ConditionRegistry : IConditionRegistry, IInitializable
    {
        private readonly Dictionary<EGlobalCondition, bool> _conditions = new();
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
            _conditions.Add(EGlobalCondition.ServModuleHasPower, false);
            _conditions.Add(EGlobalCondition.HasElectricity, false);
            _conditions.Add(EGlobalCondition.ModulePersistentClosed, false);
            // Водоснабжение для мех. модуля включено
            _conditions.Add(EGlobalCondition.MechWaterSupplySwitchedOn, true); // TRUE
            _conditions.Add(EGlobalCondition.ServerElectricitySwitchedOn, false); // FALSE

            if ((Enum.GetNames(typeof(EGlobalCondition)).Length - 1) != _conditions.Count)
                throw new Exception("Initialized conditions count mismatch!");
        }

        private void OnSwitchGlobalConditionMsg(SwitchGlobalConditionMsg msg) =>
            SwitchGlobalConditionMsg(msg.GlobalCondition);

        private void SwitchGlobalConditionMsg(EGlobalCondition type)
        {
            if (!_conditions.TryGetValue(type, out var currentValue))
                return;

            _conditions[type] = !currentValue;
            _selfPub.Publish(new GlobalConditionChangedMsg(type, _conditions[type]));
            _log.Debug("ConditionRegistry: " + type + " changed to " + _conditions[type]);
        }

        public bool IsCompleted(EGlobalCondition type) =>
            _conditions[type];

        public bool GetConditionState(EGlobalCondition type) => _conditions[type];
    }

    public interface IConditionRegistry
    {
        bool IsCompleted(EGlobalCondition type);
        bool GetConditionState(EGlobalCondition type);
    }
}
