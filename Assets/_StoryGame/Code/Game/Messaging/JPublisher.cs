using _StoryGame.Core.HSM.Interfaces;
using _StoryGame.Core.Input.Interfaces;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.UI.Interfaces;
using MessagePipe;

namespace _StoryGame.Game.Messaging
{
    public sealed class JPublisher : IJPublisher
    {
        private readonly IPublisher<IGameManagerMsg> _gameManagerPublisher;
        private readonly IPublisher<IUIViewerMsg> _uiViewerPublisher;
        private readonly IPublisher<IPlayerOverHeadUIMsg> _playerActionPublisher;
        private readonly IPublisher<IInputMsg> _inputPublisher;
        private readonly IPublisher<IPlayerAnimatorMsg> _playerAnimatorPublisher;
        private readonly IPublisher<IInteractProcessorMsg> _interactProcessorPublisher;
        private readonly IPublisher<IHSMMsg> _hsmPublisher;
        private readonly IPublisher<IRoomsDispatcherMsg> _roomsDispatcherPublisher;

        public JPublisher(
            IPublisher<IGameManagerMsg> gameManagerPublisher,
            IPublisher<IUIViewerMsg> uiViewerPublisher,
            IPublisher<IPlayerOverHeadUIMsg> playerActionPublisher,
            IPublisher<IInputMsg> inputPublisher,
            IPublisher<IPlayerAnimatorMsg> playerAnimatorPublisher,
            IPublisher<IInteractProcessorMsg> interactProcessorPublisher,
            IPublisher<IHSMMsg> hsmPublisher,
            IPublisher<IRoomsDispatcherMsg> roomsDispatcherPublisher
        )
        {
            _gameManagerPublisher = gameManagerPublisher;
            _uiViewerPublisher = uiViewerPublisher;
            _playerActionPublisher = playerActionPublisher;
            _inputPublisher = inputPublisher;
            _playerAnimatorPublisher = playerAnimatorPublisher;
            _interactProcessorPublisher = interactProcessorPublisher;
            _hsmPublisher = hsmPublisher;
            _roomsDispatcherPublisher = roomsDispatcherPublisher;
        }

        public void ForUIViewer(IUIViewerMsg msg) => _uiViewerPublisher.Publish(msg);
        public void ForGameManager(IGameManagerMsg msg) => _gameManagerPublisher.Publish(msg);
        public void ForPlayerOverHeadUI(IPlayerOverHeadUIMsg msg) => _playerActionPublisher.Publish(msg);
        public void ForInput(IInputMsg msg) => _inputPublisher.Publish(msg);
        public void ForPlayerAnimator(IPlayerAnimatorMsg msg) => _playerAnimatorPublisher.Publish(msg);
        public void ForInteractProcessor(IInteractProcessorMsg msg) => _interactProcessorPublisher.Publish(msg);
        public void ForHSM(IHSMMsg msg) => _hsmPublisher.Publish(msg);
        public void ForRoomsDispatcher(IRoomsDispatcherMsg msg) => _roomsDispatcherPublisher.Publish(msg);
    }
}
