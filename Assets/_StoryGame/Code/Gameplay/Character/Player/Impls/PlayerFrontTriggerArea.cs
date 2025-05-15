using System;
using System.Collections.Generic;
using System.Linq;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interactables.Interfaces;
using _StoryGame.Infrastructure.Localization;
using UnityEngine;
using Zenject;

namespace _StoryGame.Gameplay.Character.Player.Impls
{
    [RequireComponent(typeof(Collider))]
    public sealed class PlayerFrontTriggerArea : MonoBehaviour
    {
        [SerializeField] private LayerMask triggeredByLayer;

        [Inject] private DiContainer _container;

        private ICharacter _colliderOwner;
        private bool _isInitialized;
        private IInteractable _currentInteractable;
        private readonly HashSet<IInteractable> _interactablesInTrigger = new();
        private SignalBus _signalBus;
        private ILocalizationProvider _localizationProvider;

        public void Init(ICharacter owner)
        {
            _signalBus = _container.Resolve<SignalBus>() ?? throw new NullReferenceException("SignalBus is null.");
            _localizationProvider = _container.Resolve<ILocalizationProvider>();

            // _signalBus.Subscribe<InteractKeySignal>(OnInteractKeySignal);
            _colliderOwner = owner;
            _isInitialized = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isInitialized)
            {
                Debug.LogError("Not initialized. Use Init(). " + name);
                return;
            }

            if (!IsLayerInMask(other.gameObject.layer)) return;

            var interactable = other.gameObject.GetComponent<IInteractable>();
            if (interactable == null) return;

            _interactablesInTrigger.Add(interactable);

            if (_interactablesInTrigger.Count > 1)
            {
                var names = string.Join(", ", _interactablesInTrigger.Select(i => ((Component)i).gameObject.name));
                throw new InvalidOperationException($"Multiple interactables in trigger zone. [{names}]");
            }

            _currentInteractable = interactable;

            var position = other.transform.position;
            var promptPosition = new Vector3(position.x, 3f, position.z);
            var tip = GetInteractionTip(interactable);

            // _signalBus.Fire(new ShowInteractTipSignal(tip, promptPosition));
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_isInitialized) return;
            if (!IsLayerInMask(other.gameObject.layer)) return;

            var interactable = other.gameObject.GetComponent<IInteractable>();
            if (interactable == null) return;

            _interactablesInTrigger.Remove(interactable);

            if (interactable == _currentInteractable)
            {
                _currentInteractable = null;
                // _signalBus.Fire(new HideInteractTipSignal());
            }
        }

        // private async void OnInteractKeySignal(InteractKeySignal signal)
        // {
        //     if (!_isInitialized || _colliderOwner.State == CharacterState.Interacting || _currentInteractable == null)
        //     {
        //         Debug.LogWarning("Not initialized or already interacting. " + name);
        //         return;
        //     }
        //
        //     _colliderOwner.SetState(CharacterState.Interacting);
        //     try
        //     {
        //         await _currentInteractable.InteractAsync(_colliderOwner);
        //     }
        //     catch (Exception ex)
        //     {
        //         Debug.LogError($"Interaction failed: {ex.Message}");
        //     }
        //     finally
        //     {
        //         _colliderOwner.SetState(CharacterState.Idle);
        //     }
        // }

        private (string, string) GetInteractionTip(IInteractable interactable)
        {
            var name = _localizationProvider.Localize(interactable.LocalizationKey, WordTransform.Upper);
            var action = _localizationProvider.Localize(interactable.InteractionTipNameId, WordTransform.Upper);
            return (name, action);
        }

        private bool IsLayerInMask(int layer) => (triggeredByLayer.value & (1 << layer)) != 0;
    }
}
