using _StoryGame.Core.Input.Interfaces;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.UI.Interfaces;
using MessagePipe;

namespace _StoryGame.Game.Publisher
{
    public sealed class JPublisher : IJPublisher
    {
        private readonly IPublisher<IGameManagerMsg> _gameManagerPublisher;
        private readonly IPublisher<IUIViewerMsg> _uiViewerPublisher;
        private readonly IPublisher<IPlayerActionMsg> _playerActionPublisher;
        private readonly IPublisher<IInputMsg> _inputPublisher;

        public JPublisher(
            IPublisher<IGameManagerMsg> gameManagerPublisher,
            IPublisher<IUIViewerMsg> uiViewerPublisher,
            IPublisher<IPlayerActionMsg> playerActionPublisher,
            IPublisher<IInputMsg> inputPublisher
        )
        {
            _gameManagerPublisher = gameManagerPublisher;
            _uiViewerPublisher = uiViewerPublisher;
            _playerActionPublisher = playerActionPublisher;
            _inputPublisher = inputPublisher;
        }

        public void ForUIViewer(IUIViewerMsg msg) => _uiViewerPublisher.Publish(msg);
        public void ForGameManager(IGameManagerMsg msg) => _gameManagerPublisher.Publish(msg);
        public void ForPlayerAction(IPlayerActionMsg msg) => _playerActionPublisher.Publish(msg);
        public void ForInput(IInputMsg msg) => _inputPublisher.Publish(msg);
    }
}
