using System;
using _StoryGame.Game.Interact.Interactables.Unlock;

namespace _StoryGame.Game.Room.Abstract
{
    [Serializable]
    public struct RoomExitVo
    {
        public EExit exit;
        public UnlockableDoor door;
    }
}
