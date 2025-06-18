using System;
using System.Collections.Generic;
using System.Linq;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Loot.Interfaces;
using _StoryGame.Core.Managers;
using _StoryGame.Core.Providers.Settings;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Data.Interactable;
using _StoryGame.Data.Loot;
using _StoryGame.Data.Room;
using _StoryGame.Data.SO.Room;
using _StoryGame.Game.Interactables.Impls;
using _StoryGame.Game.Interactables.Impls.Use;
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
        [SerializeField] private RoomInteractablesVo interactables;
        [SerializeField] private List<RoomDoor> doors;

        public string Id => roomId;
        public string Name => roomName;
        public float Progress { get; }
        public RoomLootVo Loot => _roomData.Loot;
        public RoomInteractablesVo Interactables => interactables;

        private RoomData _roomData;
        private ILootSystem _lootSystem;
        private IPublisher<RoomLootGeneratedMsg> _roomLootGeneratedMsgPub;
        private readonly CompositeDisposable _disposables = new();

        private List<Conditional> _conditionalObjects = new();
        private IJLog _log;
        private IGameManager _gameManager;

        [Inject]
        private void Construct(IJLog log, ISettingsProvider settingsProvider, ILootSystem lootSystem,
            IPublisher<RoomLootGeneratedMsg> roomLootGeneratedMsgPub, AppStartHandler appStartHandler,
            IGameManager gameManager)
        {
            _log = log;
            _roomData = settingsProvider.GetRoomSettings(Id);
            _lootSystem = lootSystem;
            _roomLootGeneratedMsgPub = roomLootGeneratedMsgPub;

            _gameManager = gameManager;
            appStartHandler.IsAppStarted
                .Subscribe(OnAppStarted).AddTo(_disposables);

            _gameManager.TempWallet.OnCurrencyChanged
                .Subscribe(_ => UpdateStateForConditionalObjects()).AddTo(_disposables);
            _gameManager.PlayerWallet.OnCurrencyChanged
                .Subscribe(_ => UpdateStateForConditionalObjects()).AddTo(_disposables);

            LoadConfig();
        }

        private void Awake()
        {
            SayMyNameToObjects();

            _conditionalObjects =
                FindObjectsByType<Conditional>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

            UpdateStateForConditionalObjects();
        }

        private void OnAppStarted(Unit _)
        {
            _lootSystem.GenerateLoot(this);
            _roomLootGeneratedMsgPub.Publish(new RoomLootGeneratedMsg(roomId));
        }


        private void LoadConfig()
        {
            if (!_roomData)
                throw new NullReferenceException($"Room {Id} not found in settings.");

            if (_roomData.Id != Id)
                throw new Exception($"Room {Id} settings is not correct.");
        }

        public bool HasLoot(string inspectableId) =>
            _lootSystem.HasLoot(roomId, inspectableId);

        public InspectableLootVo GetLootData() => _roomData.Loot.inspectableLoot;

        public InspectableData GetLoot(string inspectableId) =>
            _lootSystem.GetLootForInspectable(Id, inspectableId);

        // TODO call on game start
        public bool UpdateStateForConditionalObjects()
        {
            _log.Debug("<color=green>UpdateStateForConditionalObjects</color>".ToUpper());
            if (_conditionalObjects.Count == 0)
            {
                _log.Error("Room has no conditional objects");
                return false;
            }

            foreach (var conditionalObject in _conditionalObjects)
            {
                _log.Debug($"Init state for {conditionalObject.Id}");
                var hasItem = _gameManager.IsPlayerHasItem(conditionalObject.Loot);
                _log.Debug($"Has item: {hasItem}");

                var hasConditionItems = _gameManager.IsPlayerHasConditionalItems(conditionalObject.ConditionalItems);
                _log.Debug($"Has condition items: {hasConditionItems}");

                if (hasItem)
                {
                    conditionalObject.SetConditionalState(EConditionalState.Looted);
                }
                else if (hasConditionItems)
                {
                    conditionalObject.SetConditionalState(EConditionalState.Unlocked);
                }
                else
                {
                    conditionalObject.SetConditionalState(EConditionalState.Locked);
                }
            }

            return true;
        }

        private void SayMyNameToObjects()
        {
            interactables.core.SetRoom(this);

            foreach (var interactable in interactables.hidden)
                interactable.SetRoom(this);

            foreach (var inspectable in interactables.inspectables)
                inspectable.SetRoom(this);
        }
    }
}
