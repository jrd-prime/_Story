using System;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Interfaces;
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
        private readonly IJLog _log;
        private readonly IPlayer _player;
        private NavMeshAgent _navMeshAgent;

        private readonly IMovementProcessorMsg _noneStateMsg = new MovementProcessorStateMsg(EMoveState.None);

        private readonly IMovementProcessorMsg _moveToInteractableStateMsg =
            new MovementProcessorStateMsg(EMoveState.MoveToInteractable);

        private readonly IMovementProcessorMsg _moveToPointStateMsg =
            new MovementProcessorStateMsg(EMoveState.MoveToPoint);

        private readonly IPublisher<IMovementProcessorMsg> _selfMsgPublisher;
        private readonly CompositeDisposable _disposables = new();

        public MovementProcessor(
            IJLog log,
            IPlayer player,
            IPublisher<IMovementProcessorMsg> selfMsgPublisher,
            ISubscriber<IMovementHandlerMsg> movementHandlerMsgSubscriber
        )
        {
            _log = log;
            _selfMsgPublisher = selfMsgPublisher;
            _player = player;

            movementHandlerMsgSubscriber
                .Subscribe(OnMovementHandlerMessage)
                .AddTo(_disposables);
        }

        public void Start()
        {
            _log.Info("Start MovementProcessor");
            _selfMsgPublisher.Publish(_noneStateMsg);
            _navMeshAgent = _player.NavMeshAgent;
        }


        private void MoveToInteractable(IInteractable interactable)
        {
            _log.Info("MoveToInteractable");

            _selfMsgPublisher.Publish(_moveToInteractableStateMsg);

            var entryPoint = interactable.GetEntryPoint();

            SendDestinationPoint(entryPoint);
        }

        private void MoveToPoint(Vector3 position)
        {
            _log.Info("MoveToPoint");

            _selfMsgPublisher.Publish(_moveToPointStateMsg);

            SendDestinationPoint(position);
        }

        private void SendDestinationPoint(Vector3 position) =>
            _selfMsgPublisher.Publish(new DestinationPointMsg(position));


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
                case MoveToPointHandlerMsg msg:
                    MoveToPoint(msg.Position);
                    break;
                case MoveToInteractableHandlerMsg msg:
                    MoveToInteractable(msg.Interactable);
                    break;
            }
        }

        public void Dispose() => _disposables?.Dispose();
    }

    public record DestinationPointMsg(Vector3 Position) : IMovementProcessorMsg
    {
        public string Name => nameof(DestinationPointMsg);
        public Vector3 Position { get; } = Position;
    }

    public interface IMovementProcessor
    {
    }

    public enum EMoveState
    {
        None,
        MoveToInteractable,
        MoveToPoint
    }
}
