using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Data.Interactable;
using _StoryGame.Game.Interactables.Interfaces;
using _StoryGame.Game.Room.Messages;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.AppStarter;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interactables.Abstract
{
    [RequireComponent(typeof(Collider))]
    public abstract class AInteractableBase : MonoBehaviour, IInteractable
    {
        [SerializeField] private string id;
        [SerializeField] private string objName = "Not set";
        [SerializeField] private string interactionTipNameId;
        [SerializeField] private string localizationKey;
        [SerializeField] private Transform _entrance;
        public abstract EInteractableType InteractableType { get; }
        public Vector3 GetEntryPoint() => _entrance.position;
        public IRoom Room { get; private set; }

        public bool CanInteract { get; set; } = true;
        public string InteractionTipNameId => interactionTipNameId;
        public string LocalizationKey => localizationKey;
        public string Name => objName;
        public string Id => id;

        private IPublisher<IUIViewerMsg> _uiViewerMessagePublisher;
        private string tipId;

        private readonly ReactiveCommand _command = new();
        private readonly CompositeDisposable _disposables = new();
        private IInteractable _interactableImplementation;
        private ILocalizationProvider _localizationProvider;

        protected IObjectResolver Resolver;

        // protected InteractablesTipUI InteractablesTipUI;
        private ISubscriber<RoomLootGeneratedMsg> _roomLootGeneratedMsgSub;

        [Inject]
        private void Construct(
            IObjectResolver resolver,
            AppStartHandler appStartHandler,
            ILocalizationProvider localizationProvider,
            IPublisher<IUIViewerMsg> uiViewerMessagePublisher,
            ISubscriber<RoomLootGeneratedMsg> roomLootGeneratedMsgSub)
        {
            Resolver = resolver;
            _uiViewerMessagePublisher = uiViewerMessagePublisher;
            _localizationProvider = localizationProvider;
            _roomLootGeneratedMsgSub = roomLootGeneratedMsgSub;

            appStartHandler.IsAppStarted
                .Subscribe(OnAppStarted)
                .AddTo(_disposables);
        }

        private void Awake()
        {
            if (!_entrance)
                throw new NullReferenceException($"{nameof(_entrance)} not found. {name}");

            if (string.IsNullOrEmpty(localizationKey))
                throw new NullReferenceException($"{nameof(localizationKey)} not found. {name}");

            if (string.IsNullOrEmpty(id))
                id = "id_" + localizationKey;
        }

        private void OnAppStarted(Unit _)
        {
            ResolveDependencies(Resolver);
        }

        protected virtual void ResolveDependencies(IObjectResolver resolver)
        {
        }

        public abstract UniTask InteractAsync(ICharacter character);

        public void ShowInteractionTip((string, string) interactionTip)
        {
            Debug.LogWarning("ShowInteractionTip");
            tipId = "testId";
        }

        public void SetRoom(IRoom room) =>
            Room = room;

        public void HideInteractionTip()
        {
            _disposables.Clear();

            _uiViewerMessagePublisher
                .Publish(new ResetFloatingWindowMsg(tipId));
        }

        private void OnCommand(Unit _)
        {
            HideInteractionTip();
        }
    }
}
