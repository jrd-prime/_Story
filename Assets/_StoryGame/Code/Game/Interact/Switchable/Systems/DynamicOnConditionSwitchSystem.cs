using System;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Switchable.Systems
{
    public sealed class DynamicOnConditionSwitchSystem : AInteractSystem<IDynamicSwitchable>
    {
        public DynamicOnConditionSwitchSystem(InteractSystemDepFlyweight dep) : base(dep)
        {
        }

        protected override async UniTask<bool> OnInteractAsync()
        {
            if (Interactable == null)
                throw new Exception("Interactable is null as DynamicSwitchable.");

            var conditionResult = Dep.ConditionChecker.GetConditionState(Interactable.ImpactCondition);
            var result = Interactable.InverseConditionImpact ? !conditionResult : conditionResult;

            if (result)
                return true;

            var localizedThought = Dep.L10n.Localize(Interactable.NotFulfilledThoughtKey, ETable.SmallPhrase);
            var thought = new ThoughtDataVo(localizedThought);

            Dep.Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thought));

            await UniTask.Yield();
            return true;
        }
    }
}
