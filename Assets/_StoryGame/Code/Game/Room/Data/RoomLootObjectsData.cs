using System;
using System.Collections.Generic;
using _StoryGame.Game.Interactables.Types;

namespace _StoryGame.Game.Room.Data
{
    [Serializable]
    public struct RoomLootObjectsData
    {
        /// <summary>
        /// Кор объект, который содержит основные подсказки
        /// </summary>
        public MultiStage core;

        public List<Conditional> hidden;
        public List<Inspectable> inspectables;
    }
}
