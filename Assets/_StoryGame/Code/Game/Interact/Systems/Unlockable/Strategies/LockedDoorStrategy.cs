using System;
using System.Text;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Game.Interact.Interactables;
using _StoryGame.Game.Interact.Interactables.Unlock;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Unlockable.Strategies
{
    /// <summary>
    /// Отправляем мысль
    /// </summary>
    public sealed class LockedDoorStrategy : IUnlockSystemStrategy
    {
        public string Name => nameof(LockedDoorStrategy);

        private readonly InteractSystemDepFlyweight _dep;
        private readonly ConditionChecker _conditionChecker;

        public LockedDoorStrategy(InteractSystemDepFlyweight dep, ConditionChecker conditionChecker)
        {
            _dep = dep;
            _conditionChecker = conditionChecker;
        }

        public UniTask<bool> ExecuteAsync(IUnlockable interactable)
        {
            var door = interactable as UnlockableDoor ?? throw new ArgumentException("Interactable is not a door");

            var thoughtKeys = _conditionChecker.GetKeysUnfulfilledConditions(door.UnlockConditions);
            var tho = new StringBuilder();


            foreach (var thoughtKey in thoughtKeys)
            {
                var though = _dep.LocalizationProvider.Localize(thoughtKey, ETable.SmallPhrase);
                tho.AppendLine(though);
            }

            var lockedThought = tho.ToString();

            var thought = new ThoughtDataVo(lockedThought);

            _dep.Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));
            return UniTask.FromResult(true);
        }
    }
}
