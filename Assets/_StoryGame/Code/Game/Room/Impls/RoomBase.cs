using System;
using System.Collections.Generic;
using _StoryGame.Data.SO.Room;
using _StoryGame.Game.Interactables.Impls.Use;
using _StoryGame.Infrastructure.Settings;
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

        private RoomSettings _roomSettings;

        [Inject]
        private void Construct(ISettingsProvider settingsProvider)
        {
            Debug.Log("Room Construct " + Id);
            _roomSettings = settingsProvider.GetRoomSettings(Id);

            if (!_roomSettings)
                throw new NullReferenceException($"Room {Id} not found in settings.");

            if (_roomSettings.Id != Id)
                throw new Exception($"Room {Id} settings is not correct.");
        }

        private void Awake()
        {
            LoadConfig();
            SayMyNameToObjects();
        }

        private void LoadConfig()
        {
            // load config by roomId
        }

        private void SayMyNameToObjects()
        {
            // say my name to objects
        }
    }
}
