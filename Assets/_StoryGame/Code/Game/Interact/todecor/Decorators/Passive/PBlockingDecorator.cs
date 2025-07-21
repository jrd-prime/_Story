using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Managers;
using _StoryGame.Game.Interact.todecor.Abstract;
using _StoryGame.Game.Managers.Condition;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.todecor.Decorators.Passive
{
    public sealed class PBlockingDecorator : ADecorator, IPassiveDecorator
    {
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private EGlobalCondition[] blockingConditions; // RocksBlocking или WaterSupply
        [SerializeField] private string blockMessage; // "Завален камнями" или "Лужа мешает"

        public override int Priority => 100;

        [Inject]
        private void Construct(ConditionChecker conditionManager)
        {
            // Сохранить ConditionManager, подписаться на OnConditionChanged
        }

        public UniTask<bool> ProcessPassive(IInteractable interactable)
        {
            return UniTask.FromResult(true);
        }

        private void OnConditionChanged(EGlobalCondition condition, EInteractableState state)
        {
            // Если condition в blockingConditions, вызвать ProcessPassive
        }
    }
}
