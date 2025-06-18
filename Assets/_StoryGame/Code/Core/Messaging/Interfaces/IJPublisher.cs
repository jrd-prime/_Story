using _StoryGame.Core.Input.Interfaces;
using _StoryGame.Core.UI.Interfaces;

namespace _StoryGame.Core.Messaging.Interfaces
{
    public interface IJPublisher
    {
        void ForUIViewer(IUIViewerMsg msg);
        void ForGameManager(IGameManagerMsg msg);
        void ForPlayerAction(IPlayerActionMsg msg);
        void ForInput(IInputMsg msg);
    }
}
