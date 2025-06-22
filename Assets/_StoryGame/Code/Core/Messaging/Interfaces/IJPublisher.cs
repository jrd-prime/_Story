using _StoryGame.Core.Input.Interfaces;
using _StoryGame.Core.UI.Interfaces;

namespace _StoryGame.Core.Messaging.Interfaces
{
    public interface IJPublisher
    {
        void ForUIViewer(IUIViewerMsg msg);
        void ForGameManager(IGameManagerMsg msg);
        void ForPlayerOverHeadUI(IPlayerOverHeadUIMsg msg);
        void ForInput(IInputMsg msg);
        void ForPlayerAnimator(IPlayerAnimatorMsg msg);
        void ForInteractProcessor(IInteractProcessorMsg msg);
    }
}
