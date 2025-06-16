using System;
using _StoryGame.Game.UI.Impls.Viewer.Layers.Floating;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace _StoryGame.Data.UI
{
    [Serializable]
    public struct FloatingWindowDataVo
    {
        [FormerlySerializedAs("floatingWindowType")] public EFloatingWindowType eFloatingWindowType;
        public VisualTreeAsset visualTreeAsset;
    }
}
