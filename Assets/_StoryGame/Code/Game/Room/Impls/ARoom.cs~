using System;
using System.Collections.Generic;
using _StoryGame.Core.Loot.Interfaces;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Data.Interactable;
using _StoryGame.Data.Room;
using _StoryGame.Data.SO.Room;
using _StoryGame.Game.Interactables.Impls.Use;
using _StoryGame.Game.Loot.Impls;
using _StoryGame.Infrastructure.AppStarter;
using _StoryGame.Infrastructure.Settings;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Room.Impls
{
    public abstract class RoomBase : MonoBehaviour, IRoom
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

        [Inject]
        private void Construct(ISettingsProvider settingsProvider, ILootSystem lootSystem,
            IPublisher<RoomLootGeneratedMsg> roomLootGeneratedMsgPub, AppStartHandler appStartHandler)
        {
            _roomData = settingsProvider.GetRoomSettings(Id);
            _lootSystem = lootSystem;
            _roomLootGeneratedMsgPub = roomLootGeneratedMsgPub;

            appStartHandler.IsAppStarted
                .Subscribe(OnAppStarted).AddTo(_disposables);

            LoadConfig();
        }

        private void Awake() => SayMyNameToObjects();

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

        private void SayMyNameToObjects()
        {
            interactables.core.SetRoom(this);

            foreach (var interactable in interactables.hidden)
                interactable.SetRoom(this);

            foreach (var inspectable in interactables.inspectables)
                inspectable.SetRoom(this);
        }
    }

    public record RoomLootGeneratedMsg(string RoomId)
    {
        public string RoomId { get; } = RoomId;
    }
}
