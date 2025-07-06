using System;
using _StoryGame.Core.Animations.Messages;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Messaging.Interfaces;
using MessagePipe;
using R3;
using UnityEngine;

namespace _StoryGame.Game.Character.Player.Impls
{
    public sealed class PlayerMessageHandler : IDisposable
    {
        private readonly IPlayer _player;
        private readonly CompositeDisposable _disposables = new();

        public PlayerMessageHandler(IPlayer player, ISubscriber<IPlayerAnimatorMsg> playerAnimatorMsgSub)
        {
            _player = player;
            playerAnimatorMsgSub.Subscribe(HandleMsg).AddTo(_disposables);
        }

        private void HandleMsg(IPlayerAnimatorMsg msg)
        {
            var animator = _player.Animator as Animator
                           ?? throw new NullReferenceException(nameof(_player.Animator));

            switch (msg)
            {
                case SetTriggerMsg message:
                    animator.SetTrigger(message.TriggerName);
                    break;
                case ResetTriggerMsg message:
                    animator.ResetTrigger(message.TriggerName);
                    break;
                case SetBoolMsg message:
                    animator.SetBool(message.Id, message.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(msg), msg, null);
            }
        }

        public void Dispose() => _disposables?.Dispose();
    }
}
