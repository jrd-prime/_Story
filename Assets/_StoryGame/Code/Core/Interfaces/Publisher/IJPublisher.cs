using _StoryGame.Core.Interfaces.Publisher.Messages;
using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Infrastructure.Input.Interfaces;

namespace _StoryGame.Core.Interfaces.Publisher
{
    public interface IJPublisher
    {
        void ForUIViewer(IUIViewerMsg msg);
        void ForGameManager(IGameManagerMsg msg);
        void ForPlayerAction(IPlayerActionMsg msg);
        void ForInput(IInputMsg msg);
    }
}
