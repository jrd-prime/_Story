using _StoryGame.Data.Const;
using _StoryGame.Data.Room;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.SO.Room
{
    [CreateAssetMenu(
        fileName = nameof(RoomData),
        menuName = SOPathConst.Settings + nameof(RoomData)
    )]
    public class RoomData : ASettingsBase
    {
        [SerializeField] private string roomId;
        [SerializeField] private RoomLootVo loot;
        public RoomLootVo Loot => loot;
        public string Id => roomId;
    }
}
