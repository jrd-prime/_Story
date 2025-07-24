using System;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Game.Interact.todecor.Abstract;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interact.todecor.Decorators.Active.Active
{
    public sealed class AThoughtPopupDecorator : ADecorator, IActiveDecorator
    {
        [SerializeField] private string thoughtLocalizationKey;
        [SerializeField] private bool suspendFurtherExecution;

        public override int Priority => 50;

        protected override void InitializeInternal()
        {
        }

        protected override async UniTask<EDecoratorResult> ProcessInternal(IInteractable interactable)
        {
            if (interactable == null)
                throw new Exception("Interactable is null. " + gameObject.name);

            var thought = Dep.L10n.Localize(thoughtLocalizationKey, ETable.SmallPhrase);
            var thoughtData = new ThoughtDataVo(thought);

            Dep.Publisher.ForPlayerOverHeadUI(new DisplayThoughtBubbleMsg(thoughtData));

            await UniTask.Yield();

            return suspendFurtherExecution ? EDecoratorResult.Suspend : EDecoratorResult.Success;
        }
    }
}
