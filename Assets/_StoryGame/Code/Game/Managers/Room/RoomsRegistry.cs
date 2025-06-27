using System;
using System.Collections.Generic;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Room;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Game.Room.Abstract;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Game.Managers.Room
{
    public class RoomsRegistry : MonoBehaviour, IRoomsRegistry, IInitializable
    {
        [SerializeField] private List<ARoom> rooms;

        private readonly Dictionary<ERoom, IRoom> _rooms = new();

        private IJLog _log;
        private IObjectResolver _resolver;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _resolver = resolver;
            _log = resolver.Resolve<IJLog>();
        }

        public void Initialize()
        {
        }

        private void Awake()
        {
            if (rooms == null || rooms.Count == 0)
                throw new NullReferenceException("Rooms not found.");

            foreach (var room in rooms)
            {
                _resolver.Inject(room);
                _rooms.Add(room.Type, room);
                room.Hide();
            }
        }

        public int GetRoomsCount() => rooms.Count;

        public IRoom GetRoomByType(ERoom type)
        {
            if (!_rooms.TryGetValue(type, out var room))
                throw new Exception($"Room {type} not found in registry.");

            return room;
        }
    }
}
