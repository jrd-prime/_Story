using System;
using System.Collections.Generic;
using _StoryGame.Game.Managers.Inerfaces;
using _StoryGame.Game.Room.Impls;
using _StoryGame.Infrastructure.Logging;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Game.Managers.Impls
{
    public class RoomsRegistry : MonoBehaviour, IRoomsRegistry, IInitializable
    {
        [SerializeField] private List<RoomBase> rooms;

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
                _resolver.Inject(room);
        }
    }
}
