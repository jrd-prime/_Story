using System;
using _StoryGame.Core.Managers.HSM.Impls.States;
using _StoryGame.Gameplay.UI;
using _StoryGame.Gameplay.UI.Impls;

namespace _StoryGame.Gameplay.Managers.Impls
{
    [Serializable]
    public struct UIViewData
    {
        public GameStateType type;
        public UIViewBase view;
    }
}
