using System.Collections.Generic;
using _StoryGame.Core.Providers.Settings;

namespace _StoryGame.Game.Interact.Interactables.Unlock
{
    public sealed class ConditionRegistry
    {
        private readonly Dictionary<EInteractConditionType, bool> _conditions = new();

        public ConditionRegistry()
        {
            _conditions.Add(EInteractConditionType.ServModuleHasPower, false);
            _conditions.Add(EInteractConditionType.HasElectricity, false);

            // _conditions.Add(EInteractConditionType.MechWaterSupplyOff, false);
            _conditions.Add(EInteractConditionType.MechWaterSupplyOff, true);
            
            _conditions.Add(EInteractConditionType.ModulePersistentClosed, false);
        }

        public bool IsCompleted(EInteractConditionType eInteractConditionType) =>
            _conditions[eInteractConditionType];
    }
}
