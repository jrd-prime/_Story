using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interactables.Interfaces;
using _StoryGame.Gameplay.UI.Impls;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer;

namespace _StoryGame.Gameplay.Interactables
{
    public sealed class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionTipNameId;
        [SerializeField] private string localizationKey;
        private IPublisher<IUIViewerMessage> _uiViewerMessagePublisher;

        private ReactiveCommand _command = new ReactiveCommand();
        private CompositeDisposable _disposables = new CompositeDisposable();
        private string tipId;

        [Inject]
        private void Construct(IPublisher<IUIViewerMessage> uiViewerMessagePublisher)
        {
            _uiViewerMessagePublisher = uiViewerMessagePublisher;
        }

        public bool CanInteract { get; }
        public string InteractionTipNameId => interactionTipNameId;
        public string LocalizationKey => localizationKey;

        public UniTask InteractAsync(ICharacter character)
        {
            throw new NotImplementedException();
        }

        public void ShowInteractionTip((string, string) interactionTip)
        {
            Debug.LogWarning("ShowInteractionTip");
            tipId = "testId";
            _command.Subscribe(OnCommand).AddTo(_disposables);
            _uiViewerMessagePublisher.Publish(new ShowPopUpMessage(tipId, interactionTip.Item1, _command));
        }

        public void HideInteractionTip()
        {
            _uiViewerMessagePublisher.Publish(new ResetPopUpMessage(tipId));
        }

        private void OnCommand(Unit _)
        {
            Debug.LogWarning("Command accepted. Investigate.");
        }
    }
}
