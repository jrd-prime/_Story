using _StoryGame.Data.UI;
using UnityEngine;

namespace _StoryGame.Data
{
    [CreateAssetMenu(
        fileName = nameof(UISettings),
        menuName = SOPathConst.MainSettings + nameof(UISettings),
        order = 100)]
    public sealed class UISettings : SettingsBase
    {
        public override string ConfigName => nameof(UISettings);
        public UIViewerFloatingWindowsData FloatingWindowDataVo;
    }
}
