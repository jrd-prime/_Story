using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interfaces.UI;
using _StoryGame.Game.Interactables.Data;
using _StoryGame.Game.Interactables.Interfaces;
using _StoryGame.Game.Room;
using _StoryGame.Game.UI.Impls.WorldUI;
using _StoryGame.Game.UI.Messages;
using _StoryGame.Infrastructure.AppStarter;
using _StoryGame.Infrastructure.Localization;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interactables.Abstract
{
    [RequireComponent(typeof(Collider))]
    public abstract class AInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string id;
        [SerializeField] private string objName;
        [SerializeField] private string interactionTipNameId;
        [SerializeField] private string localizationKey;
        [SerializeField] private Transform _entrance;


        public abstract EInteractableType InteractableType { get; }
        public Vector3 GetEntryPoint() => _entrance.position;
        public IRoom Room { get; private set; }

        public bool CanInteract { get; }
        public string InteractionTipNameId => interactionTipNameId;
        public string LocalizationKey => localizationKey;
        public string Name => objName;
        public string Id => id;

        private IPublisher<IUIViewerMessage> _uiViewerMessagePublisher;
        private string tipId;

        private readonly ReactiveCommand _command = new();
        private readonly CompositeDisposable _disposables = new();
        private IInteractable _interactableImplementation;
        private ILocalizationProvider _localizationProvider;
        protected IObjectResolver Resolver;
        protected InteractablesTipUI InteractablesTipUI;

        [Inject]
        private void Construct(
            IObjectResolver resolver,
            AppStartHandler appStartHandler,
            InteractablesTipUI interactablesTipUI,
            ILocalizationProvider localizationProvider,
            IPublisher<IUIViewerMessage> uiViewerMessagePublisher
        )
        {
            Resolver = resolver;
            _uiViewerMessagePublisher = uiViewerMessagePublisher;
            _localizationProvider = localizationProvider;

            InteractablesTipUI = interactablesTipUI;

            appStartHandler.IsAppStarted
                .Subscribe(OnAppStarted)
                .AddTo(_disposables);
        }

        private void Awake()
        {
            if (!_entrance)
                throw new NullReferenceException($"{nameof(_entrance)} not found. {name}");
        }

        private void OnEnable()
        {
            InteractablesTipUI.transform.SetParent(transform, false); // TODO to object pool
        }

        private void OnAppStarted(Unit _)
        {
            ResolveDependencies(Resolver);
            ShowDebug();
        }

        private void ShowDebug()
        {
            if (!InteractablesTipUI)
                throw new NullReferenceException("InteractableDebugController not found.");

            var text = _localizationProvider.LocalizeWord(LocalizationKey);
            InteractablesTipUI.SetNameText(text.ToUpper());
            InteractablesTipUI.SetType(InteractableType.ToString().ToUpper());

            SetAdditionalDebugInfo(InteractablesTipUI);
        }


        protected virtual void ResolveDependencies(IObjectResolver resolver)
        {
        }

        protected virtual void SetAdditionalDebugInfo(InteractablesTipUI deb)
        {
        }

        public abstract UniTask InteractAsync(ICharacter character);

        public void ShowInteractionTip((string, string) interactionTip)
        {
            Debug.LogWarning("ShowInteractionTip");
            tipId = "testId";
        }

        public void SetRoom(IRoom roomPrototype) =>
            Room = roomPrototype;

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
