using System;
using _StoryGame.Core.Animations.Interfaces;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Character.Player;
using _StoryGame.Core.Currency;
using _StoryGame.Gameplay.Managers.Inerfaces;
using R3;
using UnityEngine;

namespace _StoryGame.Gameplay.Character.Player.Impls
{
    public sealed class PlayerInteractor : ICharacterInteractor
    {
        public Vector3 MoveDirection { get; private set; } = Vector3.zero;
        public Camera MainCamera => _cameraManager.GetMainCamera();
        public string Id => _service.Id;
        public string Name { get; private set; }
        public string Description { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }


        private readonly PlayerService _service;
        private readonly ICameraManager _cameraManager;
        private readonly IWallet _wallet;
        private readonly IPlayerAnimationService _playerAnimationService;
        private readonly CompositeDisposable Disposables = new();

        public PlayerInteractor(PlayerService service, ICameraManager cameraManager,
            ICurrencyService currencyService
            // , 
            // IPlayerAnimationService playerAnimationService,
            // FullScreenMovementProcessor move
            )
        {
            _service = service;
            _cameraManager = cameraManager;
            // _playerAnimationService = playerAnimationService;

            _wallet = currencyService.CreateWallet("player_test_id");

            // move.MoveDirection.Subscribe(OnMoveDirectionSignal).AddTo(Disposables);
        }

        private void OnMoveDirectionSignal(Vector3 direction) => MoveDirection = direction;

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
    }

    public record MoveDirectionSignal(Vector3 Direction)
    {
        public Vector3 Direction { get; } = Direction;
    }
}
