using System;
using _StoryGame.Core.Room;
using _StoryGame.Game.Interact.Passable;

namespace _StoryGame.Game.Room.Abstract
{
    [Serializable]
    public struct RoomExitVo
    {
        public EExit exit;
        public Passable door;
    }
}
