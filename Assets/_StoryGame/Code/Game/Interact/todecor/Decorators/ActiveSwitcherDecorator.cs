using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Managers;
using _StoryGame.Game.Managers.Condition;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.todecor
{
    public sealed class ActiveSwitcherDecorator : ADecorator, IActiveDecorator
    {
        [SerializeField] private EGlobalCondition impactOnCondition; // WaterSupply
        [SerializeField] private ESwitchQuestion switchQuestion; // q_turn_on
        [SerializeField] private string requiredItem; // "Crowbar" для вентиля
        [SerializeField] private string missingItemMessage; // "Нужен лом"

        public override int Priority => 60;

        [Inject]
        private void Construct(ConditionChecker conditionManager
            // , SwitchSystem switchSystem, InventorySystem inventory
        )
        {
            // Сохранить SwitchSystem, ConditionManager, InventorySystem
        }

        public UniTask<bool> ProcessActive(IInteractable interactable)
        {
            // Проверить наличие requiredItem в InventorySystem
            // Если нет, вернуть false и missingItemMessage
            // Если есть, показать диалог (switchQuestion), вызвать SwitchSystem для impactOnCondition
            // Обновить CurrentState через interactable.SetState
             
            return UniTask.FromResult(true);
        }
    }
}
