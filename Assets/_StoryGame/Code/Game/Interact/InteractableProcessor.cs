using System;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Game.Movement;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;

namespace _StoryGame.Game.Interact
{
    public sealed class InteractableProcessor : IDisposable
    {
        public ReadOnlyReactiveProperty<string> CurrentInteractable => _currentInteractable;

        private const string DefaultInteractableValue = "-";

        private readonly IPlayer _player;
        private readonly IJLog _log;

        private readonly ReactiveProperty<string> _currentInteractable = new(DefaultInteractableValue);
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
                    msg => ProcessInteracting(msg as InteractableEntranceReachedMsg).Forget(),
                    msg => msg is InteractableEntranceReachedMsg
                ).AddTo(_disposables);
        }

        private async UniTask ProcessInteracting(InteractableEntranceReachedMsg message)
        {
            try
            {
                if (message?.Interactable == null)
                {
                    _log.Error("Invalid interactable message received");
                    return;
                }

                var interactable = message.Interactable;

                var localizedText =
                    _localizationProvider.Localize(interactable.LocalizationKey, ETable.Words, ETextTransform.Upper);
                _currentInteractable.Value = localizedText ?? DefaultInteractableValue;

                _log.Debug($"Interact Entrance Reached: {interactable}");

                if (!interactable.CanInteract)
                {
                    _log.Warn($"Can't interact with {interactable.Name}");
                    return;
                }

                _player.OnStartInteract();

                await interactable.InteractAsync(_player).SuppressCancellationThrow();

                _player.OnEndInteract();
            }
            catch (Exception ex)
            {
                _log.Error($"Error during interaction processing: {ex}");
            }
            finally
            {
                _currentInteractable.Value = DefaultInteractableValue;
            }
        }

        public void Dispose() => _disposables?.Dispose();
    }
}
