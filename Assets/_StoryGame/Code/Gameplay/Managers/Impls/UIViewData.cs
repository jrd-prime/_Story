using System;
using _StoryGame.Gameplay.UI;

namespace _StoryGame.Gameplay.Managers.Impls
{
    [Serializable]
    public struct UIViewData
    {
        public UIType type;
        public UIViewBase view;
    }

    public enum UIType
    {
        Gmaeplay,
        Menu
    }
}
