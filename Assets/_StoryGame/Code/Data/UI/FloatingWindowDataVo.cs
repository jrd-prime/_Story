using System;
using _StoryGame.Gameplay.UI.Impls.Viewer.Layers;
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
