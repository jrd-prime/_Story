using System.Collections.Generic;
using _StoryGame.Game.Room.Data;
using UnityEngine;

namespace _StoryGame.Game.Room.Impls
{
    public sealed class RoomPrototype : MonoBehaviour, IRoom
    {
        [SerializeField] private string roomName;
        [SerializeField] private float progress;
        [SerializeField] private RoomContentData roomContentData;
        [SerializeField] private List<Loot> lootPool;
        [SerializeField] private List<Transition> transitions;

        public string Name => roomName;
        public float Progress => progress;
        public List<Loot> LootPool => lootPool;

        private void Awake()
        {
            SayMyNameToObjects();
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
