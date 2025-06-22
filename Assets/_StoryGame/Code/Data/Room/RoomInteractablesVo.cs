using System;
using System.Collections.Generic;
using _StoryGame.Core.Interact;
using _StoryGame.Game.Interact.ObjTypes;

namespace _StoryGame.Data.Room
{
    [Serializable]
    public struct RoomInteractablesVo
    {
        /// <summary>
        /// Кор объект, который содержит основные подсказки
        /// </summary>
        public MultiStage core;

        public List<Conditional> hidden;
        public List<Inspectable> inspectables;

        public List<IInspectable> GetWrappedInspectables() => new(inspectables);
    }
}
