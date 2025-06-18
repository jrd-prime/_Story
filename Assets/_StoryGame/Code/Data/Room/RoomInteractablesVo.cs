using System;
using System.Collections.Generic;
using _StoryGame.Game.Interactables.Impls;
using _StoryGame.Game.Interactables.Impls.Inspect;
using _StoryGame.Game.Interactables.Interfaces;

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
