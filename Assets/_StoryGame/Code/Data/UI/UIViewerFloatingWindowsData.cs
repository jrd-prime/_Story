using UnityEngine;

namespace _StoryGame.Data.UI
{
    [CreateAssetMenu(
        fileName = nameof(UIViewerFloatingWindowsData),
        menuName = SOPathConst.UISettings + nameof(UIViewerFloatingWindowsData),
        order = 0)]
    public sealed class UIViewerFloatingWindowsData : SettingsBase
    {
        public override string ConfigName => "UIViewerFloatingWindowsData";
        public FloatingWindowDataVo[] FloatingWindowDataVo;
    }
}
