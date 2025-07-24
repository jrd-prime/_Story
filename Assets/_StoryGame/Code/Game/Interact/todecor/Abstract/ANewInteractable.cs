using System;
using System.Collections.Generic;
using System.Linq;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Core.UI.Interfaces;
using _StoryGame.Core.WalletNew.Messages;
using _StoryGame.Game.Managers.Condition;
using _StoryGame.Game.Room.Messages;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.AppStarter;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.todecor.Abstract
{
    [SelectionBase]
    [RequireComponent(typeof(Collider))]
    public abstract class ANewInteractable : MonoBehaviour, IInteractable
    {
        [Title("Interactable Base Settings", titleAlignment: TitleAlignments.Centered)]

        #region SerializeField

        [SerializeField]
        private string id;

        [SerializeField] private string objName = "Not set";
        [SerializeField] private string localizationKey;
        [SerializeField] private Transform entrancePoint;
        [Space(10)] [SerializeField] private int interactEnergyCost;

        [SerializeField] private EInteractableState initialState = EInteractableState.NotSet;
        [Space(10)] [SerializeField] private bool disableColliders;

        #endregion

        #region Public

        [ShowInInspector] [ReadOnly] public abstract EMainInteractableType interactableType { get; }
        public bool CanInteract { get; set; } = true;
        public bool IsBlocked { get; private set; }
        public EInteractableState CurrentState { get; private set; }
        public string Id => id;
        public EInteractableType InteractableType { get; }
        public Transform EntrancePoint => entrancePoint;
        public string LocalizationKey => localizationKey;
        public string Name => objName;
        public IRoom Room { get; }
        public int InteractEnergyCost => interactEnergyCost;

        #endregion

        #region Private

        protected bool _isInitialized;
        protected Collider[] _colliders;
        protected List<IPassiveDecorator> _passiveDecorators = new();
        protected List<IActiveDecorator> _activeDecorators = new();
        protected IPublisher<IUIViewerMsg> _uiViewerMessagePublisher;
        protected string _tipId;
        protected readonly ReactiveCommand _command = new();
        protected readonly CompositeDisposable _disposables = new();
        protected IInteractable _interactableImplementation;
        protected IL10nProvider _il10NProvider;
        protected IObjectResolver _resolver;
        protected IPlayer _player;
        protected ISubscriber<RoomLootGeneratedMsg> _roomLootGeneratedMsgSub;
        protected ISubscriber<ItemAmountChangedMsg> _itemLootedMsgSub;
        protected IJLog _log;
        protected IJPublisher _publisher;

        #endregion

        [Inject]
        private void Construct(
            IObjectResolver resolver,
            AppStartHandler appStartHandler,
            IL10nProvider il10NProvider,
            IPublisher<IUIViewerMsg> uiViewerMessagePublisher,
            ISubscriber<RoomLootGeneratedMsg> roomLootGeneratedMsgSub,
            ISubscriber<ItemAmountChangedMsg> itemLootedMsgSub,
            IPlayer player, // TODO remove
            IJPublisher publisher, ConditionChecker conditionChecker
        )
        {
            Debug.LogWarning("Construct called for " + gameObject.name);
            _resolver = resolver;
            _log = resolver.Resolve<IJLog>();
            _uiViewerMessagePublisher = uiViewerMessagePublisher;
            _il10NProvider = il10NProvider;
            _roomLootGeneratedMsgSub = roomLootGeneratedMsgSub;
            _itemLootedMsgSub = itemLootedMsgSub;
            _publisher = publisher;
            _player = player;

            appStartHandler.IsAppStarted
                .Subscribe(OnAppStarted)
                .AddTo(_disposables);
        }

        private void Awake()
        {
            Debug.LogWarning("Awake called for " + gameObject.name);

            Validate();

            _colliders = GetComponents<Collider>();
            OnAwake();
        }

        private void OnEnable()
        {
            if (!_isInitialized)
                return;

            Debug.LogWarning("OnEnable called for " + gameObject.name);
            UpdatePassiveState();
        }

        private void Start()
        {
            Debug.LogWarning("Start called for " + gameObject.name);
            
            if(_resolver == null)
                throw new NullReferenceException("Resolver is null. " + gameObject.name);
            
            CollectAndInjectDecorators();
            
            
            
            SetState(initialState);
            UpdatePassiveState();
            _isInitialized = true;
        }

        public async void UpdatePassiveState()
        {
            _log.Warn("--- UpdatePassiveState called for " + gameObject.name);

            IsBlocked = false;

            await ProcessPassiveDecorators();
            UpdateColliders();
        }

        private async UniTask ProcessPassiveDecorators()
        {
            _log.Warn("<color=yellow>Start Passive Decorators</color>");

            var prevState = CurrentState;
            foreach (var decorator in _passiveDecorators)
            {
                if (!decorator.IsEnabled)
                    continue;

                _log.Warn($"{decorator.GetType().Name} / {decorator.Priority}");
                var result = await decorator.Process(this);
                if (result == EDecoratorResult.Suspend)
                {
                    _log.Warn($"Suspend result from {decorator.GetType().Name} / {decorator.Priority}");
                }
            }

            if (prevState != CurrentState)
            {
                _log.Warn(
                    $"<color=cyan>Кто-то изменил состояние с {prevState} на {CurrentState}. Перезапустить процесс пассивных декораторов</color>");
                await ProcessPassiveDecorators();
            }

            _log.Warn("<color=yellow>End Passive Decorators</color>");
        }

        private async UniTask ProcessActiveDecorators()
        {
            _log.Warn("<color=green>Start Active Decorators</color>");
            var prevState = CurrentState;

            _log.Warn("ProcessActiveDecorators previous state: " + prevState + " / current state: " + CurrentState);
            foreach (var decorator in _activeDecorators)
            {
                if (!decorator.IsEnabled)
                    continue;

                _log.Warn($"{decorator.GetType().Name} / {decorator.Priority}");
                var result = await decorator.Process(this);

                if (result == EDecoratorResult.Suspend)
                {
                    _log.Warn($"Suspend result from {decorator.GetType().Name} / {decorator.Priority}");
                }

                // if (!result)
                // {
                //     // Показать сообщение (например, "Нужен лом"), остановить
                //     return;
                // }
            }

            _log.Warn("ProcessActiveDecorators previous state: " + prevState + " / current state: " + CurrentState);
            if (prevState != CurrentState)
            {
                _log.Warn(
                    $"<color=cyan>Кто-то изменил состояние с {prevState} на {CurrentState}. Перезапустить процесс пассивных декораторов</color>");
                await ProcessPassiveDecorators();
            }

            _log.Warn("<color=green>End Active Decorators</color>");
        }


        public async UniTask InteractAsync(ICharacter character)
        {
            if (IsBlocked)
                return;

            await ProcessActiveDecorators();
            UpdateColliders();
        }


        public Vector3 GetEntryPoint() => entrancePoint.position;


        public void SetRoom(IRoom room)
        {
        }

        public void SetBlocked(bool blocked)
        {
            IsBlocked = blocked;
            UpdateColliders();
        }

        public void SetState(EInteractableState state)
        {
            Debug.LogWarning("<b><color=yellow>SetState</color></b> from " + CurrentState + " to " + state +
                             " called for " + gameObject.name);
            CurrentState = state;
        }

        public void SwitchState() =>
            SetState(CurrentState == EInteractableState.On ? EInteractableState.Off : EInteractableState.On);

        private void UpdateColliders()
        {
            var result = !IsBlocked && (!disableColliders || CurrentState != EInteractableState.Off);

            Debug.Log("UpdateColliders " + result);
            foreach (var col in _colliders)
                col.enabled = result;
        }

        private void CollectAndInjectDecorators()
        {
            _passiveDecorators = GetComponents<IPassiveDecorator>()
                .OrderByDescending(decorator => decorator.Priority)
                .ToList();

            _activeDecorators = GetComponents<IActiveDecorator>()
                .OrderByDescending(decorator => decorator.Priority)
                .ToList();

            foreach (var passive in _passiveDecorators)
            {
                _resolver.Inject(passive);
                passive.Initialize();
            }

            foreach (var active in _activeDecorators)
            {
                _resolver.Inject(active);
                active.Initialize();
            }
        }

        protected virtual void OnAwake()
        {
        }

        private void OnAppStarted(Unit _)
        {
        }


        public void HideInteractionTip()
        {
            _disposables.Clear();

            _uiViewerMessagePublisher
                .Publish(new ResetFloatingWindowMsg(_tipId));
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(localizationKey))
            {
                _log.Error($"{nameof(localizationKey)} not found. {name}");
                enabled = false;
                return;
            }

            if (initialState == EInteractableState.NotSet)
            {
                _log.Error($"{nameof(initialState)} not set. {name}");
                enabled = false;
                return;
            }

            if (string.IsNullOrEmpty(id))
                id = "id_" + localizationKey;

            if (!entrancePoint)
            {
                _log.Error($"{nameof(entrancePoint)} not found. {name}");
                enabled = false;
                return;
            }
        }
    }
}
