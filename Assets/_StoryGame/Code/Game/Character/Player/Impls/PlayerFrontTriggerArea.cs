using System;
using System.Collections.Generic;
using System.Linq;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Game.Interactables.Interfaces;
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

        private ILocalizationProvider _localizationProvider;
        private IJLog _log;
        private IPublisher<IUIViewerMsg> _uiPublisher;
        private IInteractable _currentInteractable;

        private bool _isInitialized;

        private readonly HashSet<IInteractable> _interactablesInTrigger = new();

        public void Init()
        {
            if (_container == null)
                throw new NullReferenceException($"Resolver is null. {nameof(PlayerFrontTriggerArea)}");

            _log = _container.Resolve<IJLog>();
            _uiPublisher = _container.Resolve<IPublisher<IUIViewerMsg>>();
            _localizationProvider = _container.Resolve<ILocalizationProvider>();

            _isInitialized = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!CanInteract(other, out _))
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
            if (!IsInitialized() || ShouldSkipTrigger(other) ||
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
                    : _localizationProvider.Localize(interactable.LocalizationKey, ETable.Words, ETextTransform.Upper);

                action = string.IsNullOrEmpty(tip)
                    ? "Not set"
                    : _localizationProvider.Localize(interactable.InteractionTipNameId, ETable.Words,
                        ETextTransform.Upper);

                return (note, action);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get interaction tip: {ex.Message}", this);
                return (note, action);
            }
        }

        #region Conditions

        private bool IsLayerInMask(int layer) =>
            (triggeredByLayer.value & (1 << layer)) != 0;

        private bool CanProcessTrigger(Collider other) =>
            IsInitialized() && other && IsLayerInMask(other.gameObject.layer);

        private bool CanInteract(Collider other, out IInteractable interactable)
        {
            interactable = null;
            return CanProcessTrigger(other) &&
                   TryGetInteractable(other, out interactable) &&
                   _interactablesInTrigger.Add(interactable);
        }

        private bool ShouldSkipTrigger(Collider other) =>
            !_isInitialized || other == null || !IsLayerInMask(other.gameObject.layer);

        #endregion
    }
}
