using System;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Game.Movement;
using _StoryGame.Game.Movement.Messages;
using _StoryGame.Infrastructure.Logging;
using MessagePipe;
using R3;

namespace _StoryGame.Game.Interactables
{
    public sealed class InteractableProcessor : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly IPlayer _player;
        private readonly IJLog _log;

        public InteractableProcessor(
            IPlayer player,
            IJLog log,
            ISubscriber<IMovementProcessorMsg> movementProcessorMsgSub
        )
        {
            _player = player;
            _log = log;
            movementProcessorMsgSub
                .Subscribe(
                    msg => ProcessInteracting(msg as InteractableEntranceReachedMsg),
                    msg => msg is InteractableEntranceReachedMsg
                ).AddTo(_disposables);
        }

        private async void ProcessInteracting(InteractableEntranceReachedMsg message)
        {
            var interactable = message.Interactable;

            _log.Debug($"Interactable Entrance Reached Start Interact: {interactable}");

            _player.OnStartInteract();

            await interactable.InteractAsync(_player);

            _player.OnEndInteract();
        }

        public void Dispose() => _disposables?.Dispose();
    }
}
