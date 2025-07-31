using _StoryGame.Data.Room;
using _StoryGame.Game.Interact.Passable;
using UnityEngine;

namespace _StoryGame.Core.Room.Interfaces
{
    public interface IRoom
    {
        string Id { get; }
        string Name { get; }
        float Progress { get; }
        RoomLootVo Loot { get; }
        bool UpdateStateForConditionalObjects();
        Vector3 GetRoomDefSpawnPosition();
        void Hide();
        void Show();
        ERoom Type { get; }

        Passable GetExitPointFor(EExit type);
    }
}
