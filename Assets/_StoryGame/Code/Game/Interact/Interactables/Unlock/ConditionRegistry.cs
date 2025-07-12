using System.Collections.Generic;

namespace _StoryGame.Game.Interact.Interactables.Unlock
{
    public sealed class ConditionRegistry
    {
        private readonly Dictionary<InteractConditionType, bool> _conditions = new();

        public ConditionRegistry()
        {
            _conditions.Add(InteractConditionType.ServModuleHasPower, false);
            _conditions.Add(InteractConditionType.HasElectricity, false);
            
            _conditions.Add(InteractConditionType.MechWaterSupplyOff, false);
            // _conditions.Add(InteractConditionType.MechWaterSupplyOff, true);
            
            
            
            
            _conditions.Add(InteractConditionType.ModulePersistentClosed, false);
        }

        public bool IsCompleted(InteractConditionType interactConditionType) =>
            _conditions[interactConditionType];
    }
}
