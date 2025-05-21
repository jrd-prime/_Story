using System;
using _StoryGame.Core.Animations.Interfaces;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Character.Player;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Currency;
using _StoryGame.Game.Managers.Inerfaces;
using _StoryGame.Game.Movement;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.AI;

namespace _StoryGame.Game.Character.Player.Impls
{
    public sealed class PlayerInteractor : IPlayer
    {
        public ReadOnlyReactiveProperty<Vector3> DestinationPoint => _destinationPoint;
        public Camera MainCamera => _cameraManager.GetMainCamera();
        public string Id => _service.Id;
        public string Name { get; }
        public string Description { get; set; }
        public object Animator { get; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public ReactiveProperty<Vector3> Position => _playerView.Position;

        public CharacterState State { get; }
        public void SetState(CharacterState state) => _playerView.SetState(state);

        public NavMeshAgent NavMeshAgent { get; private set; }

        private readonly ReactiveProperty<Vector3> _destinationPoint = new();
        private readonly PlayerService _service;
        private readonly ICameraManager _cameraManager;
        private readonly IWallet _wallet;
        private readonly IPlayerAnimationService _playerAnimationService;
        private readonly CompositeDisposable _disposables = new();
        private readonly PlayerView _playerView;

        public PlayerInteractor(
            PlayerService service,
            PlayerView playerView,
            ICameraManager cameraManager,
            ICurrencyService currencyService,
            ISubscriber<DestinationPointMsg> destinationPointMsgSubscriber)
        {
            _playerView = playerView;
            _service = service;
            _cameraManager = cameraManager;

            destinationPointMsgSubscriber
                .Subscribe(msg => SetDestinationPoint(msg.Position))
                .AddTo(_disposables);

            // _playerAnimationService = playerAnimationService;
            _wallet = currencyService.CreateWallet("player_test_id");
        }

        /// <summary>
        /// Такое себе решение. // TODO: Подумать как лучше сделать с учетом плеера
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            if (_service == null)
                throw new NullReferenceException($"Service is null in {nameof(PlayerInteractor)}");

            _service.SetPosition(position);
        }

        public void AnimateWithTrigger(string triggerName, string animationStateName, Action onAnimationComplete)
        {
            if (_playerAnimationService == null)
                throw new NullReferenceException($"Service is null in {nameof(PlayerInteractor)}");

            _playerAnimationService.AnimateWithTrigger(triggerName, animationStateName, onAnimationComplete);
        }

        public void SetDestinationPoint(Vector3 destination)
        {
            Debug.Log($"SetDestination interactor {destination}");
            _destinationPoint.Value = destination;
        }

        public void SetNavMeshAgent(NavMeshAgent agent) => NavMeshAgent = agent;
    }
}
