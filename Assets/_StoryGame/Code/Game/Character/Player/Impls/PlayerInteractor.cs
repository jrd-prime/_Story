using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Character.Player;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Currency;
using _StoryGame.Game.Character.Player.Messages;
using _StoryGame.Game.Movement;
using _StoryGame.Game.Movement.Messages;
using _StoryGame.Infrastructure.Logging;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.AI;
using VContainer.Unity;

namespace _StoryGame.Game.Character.Player.Impls
{
    public sealed class PlayerInteractor : IPlayer, IInitializable
    {
        public ReadOnlyReactiveProperty<Vector3> Position => _playerView.Position;
        public ReadOnlyReactiveProperty<ECharacterState> State => _state;
        public ReadOnlyReactiveProperty<Vector3> DestinationPoint => _destinationPoint;
        public string Id => _service.Id;
        public string Name => _playerView.name;
        public string Description => _playerView.Description;
        public object Animator => _playerView.Animator;
        public int Health { get; set; }
        public int MaxHealth { get; set; }

        public NavMeshAgent NavMeshAgent => _playerView.NavMeshAgent;

        private readonly ReactiveProperty<ECharacterState> _state = new(ECharacterState.Idle);
        private readonly ReactiveProperty<Vector3> _destinationPoint = new();

        private readonly PlayerService _service;
        private readonly IWallet _wallet;
        private readonly CompositeDisposable _disposables = new();
        private readonly PlayerView _playerView;
        private readonly IPublisher<IPlayerMsg> _selfMsgPub;
        private readonly IJLog _log;

        public PlayerInteractor(
            PlayerService service,
            PlayerView playerView,
            IJLog log,
            ICurrencyService currencyService,
            IPublisher<IPlayerMsg> selfMsgPub)
        {
            _playerView = playerView;
            _service = service;
            _log = log;
            _selfMsgPub = selfMsgPub;
            _wallet = currencyService.CreateWallet("player_test_id");
        }


        public void Initialize()
        {
            SetState(ECharacterState.Idle);
        }

        public void SetState(ECharacterState state)
        {
            _state.Value = state;
            PublishState(state);
            Debug.LogWarning("<color=red>Player State</color> " + state);
        }

        private void PublishState(ECharacterState state) =>
            _selfMsgPub.Publish(new PlayerStateMsg(state));

        public async UniTask MoveToPointAsync(Vector3 position, EDestinationPoint destinationPointType)
        {
            Debug.Log($"SetDestination interactor {position}");
            _destinationPoint.Value = position;

            switch (destinationPointType)
            {
                case EDestinationPoint.Ground:
                    SetState(ECharacterState.MovingToPoint);
                    break;
                case EDestinationPoint.Entrance:
                    SetState(ECharacterState.MovingToInteractable);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(destinationPointType), destinationPointType, null);
            }

            await _playerView.MoveToAsync(position);

            _log.Debug($"MoveToPointAsync: {position} done");

            SetState(ECharacterState.Idle);
        }

        public void OnStartInteract()
        {
            _log.Debug("OnStartInteract: Animate & set state to Interacting");

            SetState(ECharacterState.Interacting);
        }

        public void OnEndInteract()
        {
            _log.Debug("OnEndInteract: Animate & set state to Idle");
            SetState(ECharacterState.Idle);   
        }
    }
}
