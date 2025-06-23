using _StoryGame.Core.Animations.Messages;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Msg;
using _StoryGame.Data.Animator;
using _StoryGame.Game.Movement;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Inspect.Strategies
{
    public sealed class NotInspectedStrategy : IInspectSystemStrategy
    {
        public string StrategyName => nameof(NotInspectedStrategy);

        private const float InspectDuration = 2f;
        private readonly InteractSystemDepFlyweight _dep;
        private IInspectable _inspectable;
        private readonly DialogResultHandler _dialogResultHandler;

        public NotInspectedStrategy(InteractSystemDepFlyweight dep)
        {
            _dep = dep;
            _dialogResultHandler = new DialogResultHandler(_dep.Log);
            _dialogResultHandler.AddCallback(EDialogResult.Close, OnCloseAction);
        }

        public async UniTask<bool> ExecuteAsync(IInspectable inspectable)
        {
            _inspectable = inspectable;

            return await Inspect();
        }

        private async UniTask<bool> Inspect()
        {
            var source = new UniTaskCompletionSource<EDialogResult>();
            var message = new DisplayProgressBarMsg("Inspect", InspectDuration, source);

            try
            {
                _dep.Publisher.ForPlayerAnimator(new SetBoolMsg(AnimatorConst.IsGatherHigh, true));

                _dep.Publisher.ForPlayerOverHeadUI(message);

                var result = await source.Task;

                _dep.Publisher.ForPlayerAnimator(new SetBoolMsg(AnimatorConst.IsGatherHigh, false));

                _dialogResultHandler.HandleResult(result);
            }
            finally
            {
                source?.TrySetCanceled();
            }

            return true;
        }

        private void OnCloseAction()
        {
            _inspectable.SetInspectState(EInspectState.Inspected);
            _dep.Publisher.ForInteractProcessor(new InteractRequestMsg(_inspectable));
        }
    }
}
