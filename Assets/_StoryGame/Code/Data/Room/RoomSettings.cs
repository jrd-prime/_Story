using UnityEngine;

namespace _StoryGame.Data.Room
{
    [CreateAssetMenu(
        fileName = nameof(RoomSettings),
        menuName = SOPathConst.Settings + nameof(RoomSettings)
    )]
    public class RoomSettings : ASettingsBase
    {
        public string Id { get; set; }
    }
}
