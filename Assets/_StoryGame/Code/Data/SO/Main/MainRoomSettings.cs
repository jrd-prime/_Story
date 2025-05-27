using System.Collections.Generic;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Data.SO.Room;
using UnityEngine;

namespace _StoryGame.Data.SO.Main
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
    }
}
