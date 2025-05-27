using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.UI
{
    [CreateAssetMenu(
        fileName = nameof(UIViewerFloatingWindowsData),
        menuName = SOPathConst.UISettings + nameof(UIViewerFloatingWindowsData),
        order = 0)]
    public sealed class UIViewerFloatingWindowsData : ASettingsBase
    {
        public FloatingWindowDataVo[] FloatingWindowDataVo;
    }
}
