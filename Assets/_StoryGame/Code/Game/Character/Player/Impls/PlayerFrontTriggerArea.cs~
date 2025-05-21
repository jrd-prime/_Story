using System;
using System.Collections.Generic;
using System.Linq;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Interactables.Interfaces;
using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Infrastructure.Localization;
using _StoryGame.Infrastructure.Logging;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Character.Player.Impls
{
    [RequireComponent(typeof(Collider))]
    public sealed class PlayerFrontTriggerArea : MonoBehaviour
    {
        [SerializeField] private LayerMask triggeredByLayer;

        [Inject] private IObjectResolver _container;

        private ICharacter _player;
        private ILocalizationProvider _localizationProvider;
        private IJLog _log;
        private IPublisher<IUIViewerMessage> _uiPublisher;

        private bool _isInitialized;
        private IInteractable _currentInteractable;

        private readonly HashSet<IInteractable> _interactablesInTrigger = new();

        public void Init(IPlayer player)
        {
            _player = player;

            if (player == null)
                throw new NullReferenceException($"{nameof(player)} is null. {nameof(PlayerFrontTriggerArea)}");

            if (_container == null)
                throw new NullReferenceException($"Resolver is null. {nameof(PlayerFrontTriggerArea)}");

            _log = _container.Resolve<IJLog>();
            _uiPublisher = _container.Resolve<IPublisher<IUIViewerMessage>>();
            _localizationProvider = _container.Resolve<ILocalizationProvider>();

            _isInitialized = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!CanInteractCondition(other, out _))
                return;

            UpdateCurrentInteractable();

            if (_currentInteractable == null)
                return;

            _log.Debug($"Interactable '{other.gameObject.name}' triggered.");
            _currentInteractable.ShowInteractionTip(GetInteractionTip(_currentInteractable));
        }

        private bool IsInitialized()
        {
            if (!_isInitialized)
                _log.Error("PlayerFrontTriggerArea not initialized. Use Init().");

            return _isInitialized;
        }

        private void UpdateCurrentInteractable()
        {
            _currentInteractable = _interactablesInTrigger.FirstOrDefault();

            if (_interactablesInTrigger.Count <= 1)
                return;

            var names = string.Join(", ", _interactablesInTrigger.Select(i =>
                (i as Component)?.gameObject.name ?? "Unknown"));
            _log.Debug($"Multiple interactables in trigger zone: {names}");
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsInitialized() || ShouldSkipTriggerCondition(other) ||
                !TryGetInteractable(other, out var interactable))
                return;

            if (interactable != _currentInteractable)
                _log.Warn($"Interactable {other.gameObject.name} not in trigger zone.");

            _currentInteractable.HideInteractionTip();
            _interactablesInTrigger.Remove(interactable);
        }

        private static bool TryGetInteractable(Collider other, out IInteractable interactable)
        {
            interactable = other.gameObject.GetComponent<IInteractable>();
            return interactable != null;
        }

        private (string, string) GetInteractionTip(IInteractable interactable)
        {
            if (interactable == null)
            {
                Debug.LogWarning("Interactable is null when getting interaction tip.", this);
                return (string.Empty, string.Empty);
            }

            var note = string.Empty;
            var action = string.Empty;
            try
            {
                if (_localizationProvider == null)
                    throw new NullReferenceException("Localization provider is null.");

                var key = interactable.LocalizationKey;
                var tip = interactable.InteractionTipNameId;

                note = string.IsNullOrEmpty(key)
                    ? "Not set"
                    : _localizationProvider.LocalizeWord(interactable.LocalizationKey, WordTransform.Upper);

                action = string.IsNullOrEmpty(tip)
                    ? "Not set"
                    : _localizationProvider.LocalizeWord(interactable.InteractionTipNameId, WordTransform.Upper);

                return (note, action);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get interaction tip: {ex.Message}", this);
                return (note, action);
            }
        }

        #region Conditions

        private bool IsLayerInMaskCondition(int layer) =>
            (triggeredByLayer.value & (1 << layer)) != 0;

        private bool CanProcessTriggerCondition(Collider other) =>
            IsInitialized() && other && IsLayerInMaskCondition(other.gameObject.layer);

        private bool CanInteractCondition(Collider other, out IInteractable interactable)
        {
            interactable = null;
            return CanProcessTriggerCondition(other) &&
                   TryGetInteractable(other, out interactable) &&
                   _interactablesInTrigger.Add(interactable);
        }

        private bool ShouldSkipTriggerCondition(Collider other) =>
            !_isInitialized || other == null || !IsLayerInMaskCondition(other.gameObject.layer);

        #endregion
    }
}
