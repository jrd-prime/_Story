using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interactables.Interfaces;
using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.UI.Messages;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interactables
{
    public sealed class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionTipNameId;
        [SerializeField] private string localizationKey;

        public bool CanInteract { get; }
        public string InteractionTipNameId => interactionTipNameId;
        public string LocalizationKey => localizationKey;

        private IPublisher<IUIViewerMessage> _uiViewerMessagePublisher;
        private string tipId;

        private readonly ReactiveCommand _command = new();
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(IPublisher<IUIViewerMessage> uiViewerMessagePublisher)
        {
            _uiViewerMessagePublisher = uiViewerMessagePublisher;
        }


        public UniTask InteractAsync(ICharacter character)
        {
            throw new NotImplementedException();
        }

        public void ShowInteractionTip((string, string) interactionTip)
        {
            Debug.LogWarning("ShowInteractionTip");
            tipId = "testId";
            _command.Subscribe(OnCommand).AddTo(_disposables);
            _uiViewerMessagePublisher.Publish(new ShowFloatingWindowMessage(tipId, interactionTip.Item1, _command));
        }

        public void HideInteractionTip()
        {
            _disposables.Clear();
            _uiViewerMessagePublisher.Publish(new ResetFloatingWindowMessage(tipId));
        }

        private void OnCommand(Unit _)
        {
            HideInteractionTip();
        }
    }
}
