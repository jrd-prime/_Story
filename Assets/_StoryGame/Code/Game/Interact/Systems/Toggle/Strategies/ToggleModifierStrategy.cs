using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.UI;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Game.Interact.Interactables.Condition;
using _StoryGame.Game.Interact.Interactables.Unlock;
using _StoryGame.Game.Interact.Systems.Inspect;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.Interact;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Game.Interact.Systems.Toggle
{
    public class ToggleModifierStrategy : IToggleSystemStrategy
    {
        public string Name => nameof(ToggleModifierStrategy);

        private ESwitchState _switchState;

        private readonly InteractSystemDepFlyweight _dep;
        private readonly ConditionChecker _conditionChecker;
        private readonly DialogResultHandler _dialogResultHandler;

        public ToggleModifierStrategy(InteractSystemDepFlyweight dep, ConditionChecker conditionChecker)
        {
            _dep = dep;
            _dialogResultHandler = new DialogResultHandler(_dep.Log);
            _dialogResultHandler.AddCallback(EDialogResult.Yes, OnYesAction);
            _dialogResultHandler.AddCallback(EDialogResult.No, OnNoAction);
        }


        private void OnYesAction()
        {
            _dep.Log.Warn("ToggleModifierStrategy.OnYesAction");
        }

        private void OnNoAction()
        {
            _dep.Log.Warn("ToggleModifierStrategy.OnNoAction");
        }

        public async UniTask<bool> ExecuteAsync(IToggleable interactable)
        {
            _dep.Publisher.ForUIViewer(new CurrentOperationMsg("ToggleModifierStrategy"));


            var source = new UniTaskCompletionSource<EDialogResult>();
            var title = "ToggleModifierStrategy";
            var state = "state";
            var question = "question";
            var cost = 1;

            var message = new ShowDialogWindowMsg(title, state, question, cost, source);

            try
            {
                _dep.Publisher.ForUIViewer(message);

                _dep.Log.Warn("pre await source.Task");
                var result = await source.Task;
                _dep.Log.Warn("post await source.Task");
                source = null;

                _dialogResultHandler.HandleResult(result);
            }
            finally
            {
                source?.TrySetCanceled();
            }

            return true;
        }

        public void SetState(ESwitchState switchState) => _switchState = switchState;
    }

    public record ShowDialogWindowMsg(
        string Title,
        string State,
        string Question,
        int Cost,
        UniTaskCompletionSource<EDialogResult> CompletionSource
    ) : IUIViewerMsg;
}
