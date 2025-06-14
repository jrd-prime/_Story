using System;
using _StoryGame.Core.HSM.Impls.States;
using _StoryGame.Game.UI.Abstract;

namespace _StoryGame.Data.UI
{
    [Serializable]
    public struct UIViewData
    {
        public GameStateType type;
        public AUIViewBase view;
    }
}
