using System;
using System.Collections;
using System.Collections.Generic;
using _StoryGame.Data.Room;
using _StoryGame.Data.SO.Room;
using _StoryGame.Game.Interactables.Impls.Use;
using _StoryGame.Game.Loot;
using _StoryGame.Infrastructure.Settings;
using MessagePipe;
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

        public List<LootType> GetLootFor(string id)
        {
            return _lootSystem.GetLootFor(roomId, id);
        }

        private RoomData _roomData;
        private ILootSystem _lootSystem;
        private IPublisher<RoomLootGeneratedMsg> _roomLootGeneratedMsgPub;

        [Inject]
        private void Construct(ISettingsProvider settingsProvider, ILootSystem lootSystem,
            IPublisher<RoomLootGeneratedMsg> roomLootGeneratedMsgPub)
        {
            Debug.Log("Room Construct " + Id);
            _roomData = settingsProvider.GetRoomSettings(Id);
            _lootSystem = lootSystem;
            _roomLootGeneratedMsgPub = roomLootGeneratedMsgPub;

            LoadConfig();
        }

        private void Awake()
        {
            SayMyNameToObjects();
        }

        private void OnEnable()
        {
            if (_lootSystem.GenerateLoot(this))
                _roomLootGeneratedMsgPub.Publish(new RoomLootGeneratedMsg(roomId));
        }

        private void LoadConfig()
        {
            if (!_roomData)
                throw new NullReferenceException($"Room {Id} not found in settings.");

            if (_roomData.Id != Id)
                throw new Exception($"Room {Id} settings is not correct.");
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

    public record RoomLootGeneratedMsg(string RoomId)
    {
        public string RoomId { get; } = RoomId;
    }
}
