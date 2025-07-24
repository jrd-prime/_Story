using System;
using System.Collections.Generic;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Interact;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Managers;
using _StoryGame.Core.Providers.Settings;
using _StoryGame.Core.Room;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Data.Interact;
using _StoryGame.Data.Room;
using _StoryGame.Data.SO.Room;
using _StoryGame.Game.Interact.Passable;
using _StoryGame.Game.Interact.SortMbDelete.InteractablesSORT;
using _StoryGame.Game.Interact.todecor;
using _StoryGame.Game.Room.Messages;
using _StoryGame.Infrastructure.AppStarter;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Room.Abstract
{
    public abstract class ARoom : MonoBehaviour, IRoom
    {
        [SerializeField] private string roomId;
        [SerializeField] private string roomName;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private ERoom type;
        [SerializeField] private RoomExitVo[] exits;
        [SerializeField] private Toggleable[] toggleables;
        [SerializeField] private Effectable[] effectables;

        public ERoom Type => type;
        public string Id => roomId;
        public string Name => roomName;
        public float Progress { get; }
        public RoomLootVo Loot => _roomData.Loot;

        private RoomData _roomData;
        private IPublisher<RoomLootGeneratedMsg> _roomLootGeneratedMsgPub;
        private IJLog _log;
        private IGameManager _gameManager;

        private readonly CompositeDisposable _disposables = new();
        private readonly Dictionary<EExit, Passable> _exitDoors = new(); // <exit, door>
        private readonly List<IPassable> _conditionalObjects = new();
        private IObjectResolver _resolver;

        [Inject]
        private void Construct(IJLog log, ISettingsProvider settingsProvider,
            IPublisher<RoomLootGeneratedMsg> roomLootGeneratedMsgPub, AppStartHandler appStartHandler,
            IGameManager gameManager, IObjectResolver resolver)
        {
            _log = log;
            _resolver = resolver;
            _roomData = settingsProvider.GetRoomSettings(Id);
            _roomLootGeneratedMsgPub = roomLootGeneratedMsgPub;

            _gameManager = gameManager;
            appStartHandler.IsAppStarted
                .Subscribe(OnAppStarted).AddTo(_disposables);

            // _gameManager.TempWallet.OnCurrencyChanged
            //     .Subscribe(_ => UpdateStateForConditionalObjects()).AddTo(_disposables);
            // _gameManager.PlayerWallet.OnCurrencyChanged
            //     .Subscribe(_ => UpdateStateForConditionalObjects()).AddTo(_disposables);

            LoadConfig();
        }

        private void Awake()
        {
            var conditionals = FindObjectsByType<Passable>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            _conditionalObjects.AddRange(conditionals);

            foreach (var exit in exits)
                _exitDoors.TryAdd(exit.exit, exit.door);

            foreach (var effectable in effectables)
                _resolver.Inject(effectable);

            // UpdateStateForConditionalObjects();

            // foreach (var toggleable in toggleables)
            //     toggleable.InitOnRoomAwake();

            var interactables =
                FindObjectsByType<Switchable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var interactable in interactables)
                _resolver.Inject(interactable);
        }

        private void Start()
        {
            _log.Warn("Room started. " + gameObject.name);
        }

        private async void OnAppStarted(Unit _)
        {
            _roomLootGeneratedMsgPub.Publish(new RoomLootGeneratedMsg(roomId));
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (exits == null || exits.Length == 0)
                throw new Exception("Exits is null or empty. " + name);
        }

#endif

        public Passable GetExitPointFor(EExit exitType)
        {
            if (!_exitDoors.TryGetValue(exitType, out var exit))
                throw new Exception($"Exit to {exitType} not found in room {Id} {Name}.");

            return exit;
        }

        private void LoadConfig()
        {
            if (!_roomData)
                throw new NullReferenceException($"Room {Id} not found in settings.");

            if (_roomData.Id != Id)
                throw new Exception($"Room {Id} settings is not correct.");
        }

        public InspectableLootVo GetLootData() => _roomData.Loot.inspectableLoot;

        // TODO call on game start
        public bool UpdateStateForConditionalObjects()
        {
            _log.Debug("<color=green>Update State For Conditional Objects</color>".ToUpper());

            if (_conditionalObjects.Count == 0)
            {
                // _log.Error("Room has no conditional objects");
                return false;
            }

            return true;
        }

        public Vector3 GetRoomDefSpawnPosition() => spawnPoint.position;

        public void Hide()
        {
            // _log.Info("Hide " + name);
            gameObject.SetActive(false);
        }

        public void Show()
        {
            // _log.Info("Show " + name);
            gameObject.SetActive(true);
        }
    }
}
