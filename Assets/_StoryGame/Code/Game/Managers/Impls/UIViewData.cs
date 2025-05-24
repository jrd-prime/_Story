using System;
using _StoryGame.Core.HSM.Impls.States;
using _StoryGame.Game.UI.Impls;

namespace _StoryGame.Game.Managers.Impls
{
    [Serializable]
    public struct UIViewData
    {
        public GameStateType type;
        public UIViewBase view;
    }
}
