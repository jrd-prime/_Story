using System.Collections.Generic;
using _StoryGame.Data.Room;
using _StoryGame.Game.Room.Data;
using _StoryGame.Infrastructure.Settings;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Room.Impls
{
    public sealed class RoomPrototype : MonoBehaviour, IRoom
    {
        [SerializeField] private string roomId;
        [SerializeField] private string roomName;
        [SerializeField] private float progress;
        [SerializeField] private RoomContentData roomContentData;
        [SerializeField] private List<Loot> lootPool;
        [SerializeField] private List<Transition> transitions;


        public string Name => roomName;
        public float Progress => progress;
        public List<Loot> LootPool => lootPool;

        private RoomSettings _roomSettings;

        [Inject]
        private void Construct(ISettingsProvider settingsProvider)
        {
            Debug.Log("Room Construct " + roomId);
            _roomSettings = settingsProvider.GetRoomSettings(roomId);
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
            roomContentData.lootObjects.core.SetRoom(this);

            foreach (var conditional in roomContentData.lootObjects.hidden)
                conditional.SetRoom(this);

            foreach (var inspectable in roomContentData.lootObjects.inspectables)
                inspectable.SetRoom(this);
        }
    }
}
