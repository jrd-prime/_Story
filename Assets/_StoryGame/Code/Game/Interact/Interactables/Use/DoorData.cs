using System;
using _StoryGame.Core.Room;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Game.Interact.Interactables.Usable
{
    [CreateAssetMenu(
        fileName = "DoorData",
        menuName = SOPathConst.Interactables + nameof(DoorData),
        order = 0
    )]
    public sealed class DoorData : ASettingsBase
    {
        public ERoom fromRoom = ERoom.NotSet;
        public ERoom toRoom = ERoom.NotSet;
        public EDoorRotation doorRotation = EDoorRotation.NotSet;
        public EDoorAction doorAction = EDoorAction.NotSet;

        private void OnValidate()
        {
            if (fromRoom == ERoom.NotSet)
                throw new Exception("From room not set " + name);
            if (toRoom == ERoom.NotSet)
                throw new Exception("To room not set " + name);
            if (doorRotation == EDoorRotation.NotSet)
                throw new Exception("Door rotation not set " + name);
            if (doorAction == EDoorAction.NotSet)
                throw new Exception("Door action not set " + name);
        }
    }
}
