using System;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Interactables.Interfaces;
using _StoryGame.Infrastructure.Logging;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.AI;
using VContainer.Unity;

namespace _StoryGame.Game.Movement
{
    public sealed class MovementProcessor : IMovementProcessor, IStartable, IDisposable
    {
        private NavMeshAgent _navMeshAgent => _player.NavMeshAgent;

        private readonly IMovementProcessorMsg _noneStateMsg = new MovementProcessorStateMsg(EMoveState.None);

        private readonly IMovementProcessorMsg
            _toInteractStateMsg = new MovementProcessorStateMsg(EMoveState.ToInteract);

        private readonly IMovementProcessorMsg _toPointStateMsg = new MovementProcessorStateMsg(EMoveState.ToPoint);

        private readonly IJLog _log;
        private readonly IPlayer _player;
        private readonly IPublisher<IMovementProcessorMsg> _selfMsgPub;

        private readonly CompositeDisposable _disposables = new();

        public MovementProcessor(
            IJLog log,
            IPlayer player,
            IPublisher<IMovementProcessorMsg> selfMsgPub,
            ISubscriber<IMovementHandlerMsg> movementHandlerMsgSub
        )
        {
            _log = log;
            _selfMsgPub = selfMsgPub;
            _player = player;

            movementHandlerMsgSub
                .Subscribe(OnMovementHandlerMessage)
                .AddTo(_disposables);
        }

        public void Start()
        {
            _log.Info("Start MovementProcessor");
            _selfMsgPub.Publish(_noneStateMsg);
        }


        private void MoveToInteractable(IInteractable interactable)
        {
            _log.Info("MoveToInteractable");

            _selfMsgPub.Publish(_toInteractStateMsg);

            var entryPoint = interactable.GetEntryPoint();

            SendDestinationPoint(entryPoint);

            _selfMsgPub.Publish(_noneStateMsg);
        }

        private void MoveToPoint(Vector3 position)
        {
            _log.Debug("MoveToPoint: " + position);

            _selfMsgPub.Publish(_toPointStateMsg);

            SendDestinationPoint(position);
        }

        private void SendDestinationPoint(Vector3 position) =>
            _selfMsgPub.Publish(new DestinationPointMsg(position));


        // public void Tick()
        // {
        //     // Проверяем, достиг ли агент цели
        //     if (_moveState.Value != EMoveState.None && _navMeshAgent != null)
        //     {
        //         if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        //         {
        //             if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
        //             {
        //                 // Цель достигнута
        //                 if (_moveState.Value == EMoveState.MoveToInteractable && _currentInteractable != null)
        //                 {
        //                     _log.Info($"Reached interactable: {_currentInteractable.Name}");
        //                     _currentInteractable.Interact();
        //                     _currentInteractable = null; // Сбрасываем после взаимодействия
        //                 }
        //                 _moveState.Value = EMoveState.None; // Сбрасываем состояние
        //             }
        //         }
        //     }
        // }

        private void OnMovementHandlerMessage(IMovementHandlerMsg message)
        {
            switch (message)
            {
                case MoveToInteractableHandlerMsg msg:
                    MoveToInteractable(msg.Interactable);
                    break;
                case MoveToPointHandlerMsg msg:
                    MoveToPoint(msg.Position);
                    break;
                default:
                    _log.Warn($"Unknown message: {message.GetType().Name}");
                    break;
            }
        }

        public void Dispose() => _disposables?.Dispose();
    }

    public enum EMoveState
    {
        None,
        ToInteract,
        ToPoint
    }
}
