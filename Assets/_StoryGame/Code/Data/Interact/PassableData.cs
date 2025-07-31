using System;
using _StoryGame.Core.Room;
using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Game.Interact.SortMbDelete.InteractablesSORT;
using UnityEngine;

namespace _StoryGame.Data.Interact
{
    [CreateAssetMenu(
        fileName = "PassableData",
        menuName = SOPathConst.Interactables + nameof(PassableData),
        order = 0
    )]
    public sealed class PassableData : ASettingsBase
    {
        public EExit exit = EExit.NotSet;
        public ERoom fromRoom = ERoom.NotSet;
        public ERoom toRoom = ERoom.NotSet;
        public EDoorRotation doorRotation = EDoorRotation.NotSet;
        public EDoorAction doorAction = EDoorAction.NotSet;
        public int usePrice = 2;

        private void OnValidate()
        {
            if (exit == EExit.NotSet)
                throw new Exception("Exit type not set " + name);
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
