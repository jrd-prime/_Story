using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interactables.Interfaces;
using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.UI.Impls.WorldUI;
using _StoryGame.Game.UI.Messages;
using _StoryGame.Infrastructure.AppStarter;
using _StoryGame.Infrastructure.Localization;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interactables
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string objName;
        [SerializeField] private string interactionTipNameId;
        [SerializeField] private string localizationKey;
        [SerializeField] private Transform _entrance;

        public abstract EInteractableType InteractableType { get; }
        public Vector3 GetEntryPoint() => _entrance.position;

        public bool CanInteract { get; }
        public string InteractionTipNameId => interactionTipNameId;
        public string LocalizationKey => localizationKey;
        public string Name => objName;

        private IPublisher<IUIViewerMessage> _uiViewerMessagePublisher;
        private string tipId;

        private readonly ReactiveCommand _command = new();
        private readonly CompositeDisposable _disposables = new();
        private IInteractable _interactableImplementation;
        private ILocalizationProvider _localizationProvider;

        [Inject]
        private void Construct(AppStartHandler appStartHandler, ILocalizationProvider localizationProvider,
            IPublisher<IUIViewerMessage> uiViewerMessagePublisher)
        {
            _uiViewerMessagePublisher = uiViewerMessagePublisher;
            _localizationProvider = localizationProvider;

            appStartHandler.IsAppStarted
                .Subscribe(OnAppStarted)
                .AddTo(_disposables);
        }

        private void OnAppStarted(Unit _)
        {
            ShowDebug();
        }

        private void ShowDebug()
        {
            InteractablesTipUI deb = gameObject.GetComponentInChildren<InteractablesTipUI>();

            if (!deb)
                throw new NullReferenceException("InteractableDebugController not found.");

            var text = _localizationProvider.LocalizeWord(LocalizationKey);
            deb.SetNameText(text.ToUpper());
            deb.SetType(InteractableType.ToString().ToUpper());

            SetAdditionalDebugInfo(deb);
        }

        protected virtual void SetAdditionalDebugInfo(InteractablesTipUI deb)
        {
        }

        public abstract UniTask InteractAsync(ICharacter character);

        public void ShowInteractionTip((string, string) interactionTip)
        {
            Debug.LogWarning("ShowInteractionTip");
            tipId = "testId";

            _command
                .Subscribe(OnCommand)
                .AddTo(_disposables);

            _uiViewerMessagePublisher
                .Publish(new ShowFloatingWindowMessage(tipId, interactionTip.Item1, _command));
        }

        public void HideInteractionTip()
        {
            _disposables.Clear();

            _uiViewerMessagePublisher
                .Publish(new ResetFloatingWindowMessage(tipId));
        }

        private void OnCommand(Unit _)
        {
            HideInteractionTip();
        }
    }
}
