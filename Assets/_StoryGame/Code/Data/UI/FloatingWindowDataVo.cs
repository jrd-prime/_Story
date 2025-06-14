using System;
using _StoryGame.Game.UI.Impls.Viewer.Layers.Floating;
using UnityEngine.UIElements;

namespace _StoryGame.Data.UI
{
    [Serializable]
    public struct FloatingWindowDataVo
    {
        public FloatingWindowType floatingWindowType;
        public VisualTreeAsset visualTreeAsset;
    }
}
