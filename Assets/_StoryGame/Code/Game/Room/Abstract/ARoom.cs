using System;
using System.Collections.Generic;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Loot.Interfaces;
using _StoryGame.Core.Managers;
using _StoryGame.Core.Providers.Settings;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Data.Interact;
using _StoryGame.Data.Loot;
using _StoryGame.Data.Room;
using _StoryGame.Data.SO.Room;
using _StoryGame.Game.Interact.Interactables;
using _StoryGame.Game.Interact.Interactables.Usable;
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
        [SerializeField] private List<Exit> doors;

        public string Id => roomId;
        public string Name => roomName;
        public float Progress { get; }
        public RoomLootVo Loot => _roomData.Loot;
        public RoomInteractablesVo Interactables => interactables;

        private RoomData _roomData;
        private ILootSystem _lootSystem;
        private IPublisher<RoomLootGeneratedMsg> _roomLootGeneratedMsgPub;
        private readonly CompositeDisposable _disposables = new();

        private readonly List<IConditional> _conditionalObjects = new();
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

            var conditionals =
                FindObjectsByType<Conditional>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            _conditionalObjects.AddRange(conditionals);

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
            _log.Debug("<color=green>Update State For Conditional Objects</color>".ToUpper());

            if (_conditionalObjects.Count == 0)
            {
                _log.Error("Room has no conditional objects");
                return false;
            }

            foreach (var conditionalObject in _conditionalObjects)
            {
                var hasItem = _gameManager.IsPlayerHasItem(conditionalObject.Loot);
                var hasConditionItems = _gameManager.IsPlayerHasConditionalItems(conditionalObject.ConditionalItems);

                if (hasItem)
                    conditionalObject.SetConditionalState(EConditionalState.Looted);
                else if (hasConditionItems)
                    conditionalObject.SetConditionalState(EConditionalState.Unlocked);
                else
                    conditionalObject.SetConditionalState(EConditionalState.Locked);

                _log.Debug(
                    $"Init state for {conditionalObject.Id} / Player already has item: {hasItem} / Has items for unlock: {hasConditionItems}");
            }

            return true;
        }

        private void SayMyNameToObjects()
        {
            interactables.core.SetRoom(this);

            foreach (var conditional in interactables.hidden)
                conditional.SetRoom(this);

            foreach (var inspectable in interactables.inspectables)
                inspectable.SetRoom(this);
        }
    }
}
