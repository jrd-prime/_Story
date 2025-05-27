using System;
using System.Collections.Generic;
using _StoryGame.Data.Room;
using UnityEngine;

namespace _StoryGame.Data.Main
{
    [CreateAssetMenu(
        fileName = nameof(MainRoomSettings),
        menuName = SOPathConst.MainSettings + nameof(MainRoomSettings)
    )]
    public class MainRoomSettings : ASettingsBase
    {
        public List<RoomSettings> Rooms;

        private readonly Dictionary<string, RoomSettings> _roomSettings = new(); // <room id , room settings>

        private void Awake()
        {
            foreach (var room in Rooms)
                _roomSettings.TryAdd(room.Id, room);
        }

        public RoomSettings GetRoomSettings(string roomId)
        {
            if (!_roomSettings.TryGetValue(roomId, out var settings))
                throw new Exception($"Room {roomId} not found in cache.");

            return settings;
        }
    }
}
