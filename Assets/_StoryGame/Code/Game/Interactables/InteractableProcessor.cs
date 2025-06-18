using System;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Game.Movement;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using UnityEngine;

namespace _StoryGame.Game.Interactables
{
    public sealed class InteractableProcessor : IDisposable
    {
        public ReadOnlyReactiveProperty<string> CurrentInteractable => _currentInteractable;

        private readonly IPlayer _player;
        private readonly IJLog _log;

        private readonly ReactiveProperty<string> _currentInteractable = new("-");
        private readonly CompositeDisposable _disposables = new();
        private readonly ILocalizationProvider _localizationProvider;

        public InteractableProcessor(
            IPlayer player,
            IJLog log,
            ILocalizationProvider localizationProvider,
            ISubscriber<IMovementProcessorMsg> movementProcessorMsgSub
        )
        {
            _player = player;
            _log = log;
            _localizationProvider = localizationProvider;
            movementProcessorMsgSub
                .Subscribe(
                    msg => ProcessInteracting(msg as InteractableEntranceReachedMsg),
                    msg => msg is InteractableEntranceReachedMsg
                ).AddTo(_disposables);
        }

        private async UniTask ProcessInteracting(InteractableEntranceReachedMsg message)
        {
            var interactable = message.Interactable;

            _currentInteractable.Value =
                _localizationProvider.Localize(interactable.LocalizationKey, ETable.Words, ETextTransform.Upper);

            _log.Debug($"Interactable Entrance Reached Start Interact: {interactable}");

            if (!interactable.CanInteract)
            {
                Debug.LogWarning("Can't interact with " + interactable.Name);
                return;
            }

            _player.OnStartInteract();

            await interactable.InteractAsync(_player);

            _player.OnEndInteract();
            
            _currentInteractable.Value = "-";
        }

        public void Dispose() => _disposables?.Dispose();
    }
}
