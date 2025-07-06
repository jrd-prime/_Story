using System;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Game.Movement.Messages;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using UnityEngine;

namespace _StoryGame.Game.Movement
{
    public sealed class MovementProcessor : IDisposable
    {
        private readonly IJLog _log;
        private readonly IPlayer _player;
        private readonly IPublisher<IInteractProcessorMsg> _selfMsgPub;

        private readonly CompositeDisposable _disposables = new();

        public MovementProcessor(
            IJLog log,
            IPlayer player,
            IPublisher<IInteractProcessorMsg> selfMsgPub,
            ISubscriber<IMovementHandlerMsg> movementHandlerMsgSub
        )
        {
            _log = log;
            _selfMsgPub = selfMsgPub;
            _player = player;

            movementHandlerMsgSub
                .Subscribe(OnMovementHandlerMsg)
                .AddTo(_disposables);
        }

        private async UniTask MoveToInteractable(IInteractable interactable)
        {
            var entryPoint = interactable.GetEntryPoint();

            await _player.MoveToPointAsync(entryPoint, EDestinationPoint.Entrance);
            // _log.Debug($"MoveToInteractable: {entryPoint} done");

            _selfMsgPub.Publish(new InteractRequestMsg(interactable));
        }

        private async UniTask MoveToPoint(Vector3 position)
        {
            await _player.MoveToPointAsync(position, EDestinationPoint.Ground);
            // _log.Debug($"MoveToPoint: {position} done");
        }

        private void OnMovementHandlerMsg(IMovementHandlerMsg message)
        {
            // _log.Debug($"OnMovementHandlerMsg: {message.GetType().Name}");
            switch (message)
            {
                case MoveToInteractableHandlerMsg msg:
                    MoveToInteractable(msg.Interactable).Forget();
                    break;
                case MoveToPointHandlerMsg msg:
                    MoveToPoint(msg.Position).Forget();
                    break;
                default:
                    _log.Warn($"Unknown message: {message.GetType().Name}");
                    break;
            }
        }

        public void Dispose() => _disposables?.Dispose();
    }

    public record InteractRequestMsg(IInteractable Interactable) : IInteractProcessorMsg
    {
        public IInteractable Interactable { get; } = Interactable;
    }
}
