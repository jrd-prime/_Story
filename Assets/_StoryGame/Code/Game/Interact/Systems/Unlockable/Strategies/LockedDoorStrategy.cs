using System;
using System.Text;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Game.Interact.Interactables;
using _StoryGame.Game.Interact.Interactables.Unlock;
using _StoryGame.Game.Movement;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Unlockable.Strategies
{
    /// <summary>
    /// Если условия не выполнены - показываем подсказку. Если выполнены - меняем состояние и вызываем реинтеракт
    /// </summary>
    public sealed class LockedDoorStrategy : IPassSystemStrategy
    {
        public string Name => nameof(LockedDoorStrategy);

        private readonly InteractSystemDepFlyweight _dep;
        private readonly ConditionChecker _conditionChecker;

        public LockedDoorStrategy(InteractSystemDepFlyweight dep, ConditionChecker conditionChecker)
            => (_dep, _conditionChecker) = (dep, conditionChecker);

        public async UniTask<bool> ExecuteAsync(IUnlockable interactable)
        {
            var door = interactable as Passable
                       ?? throw new ArgumentException("Interactable is not a door");

            var result = _conditionChecker.GetKeysUnfulfilledConditions(door.ConditionsData);

            if (result.Success)
            {
                door.SetState(EDoorState.Unlocked);
                await UniTask.Yield();
                _dep.Publisher.ForInteractProcessor(new InteractRequestMsg(door));
            }
            else
            {
                var localizedThoughtsBuilder = new StringBuilder();

                foreach (var thoughtKey in result.Toughts)
                    localizedThoughtsBuilder.AppendLine("Line / " + _dep.L10n.Localize(thoughtKey, ETable.SmallPhrase));

                var thought = new ThoughtDataVo(localizedThoughtsBuilder.ToString());

                _dep.Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));
            }

            return true;
        }
    }
}
