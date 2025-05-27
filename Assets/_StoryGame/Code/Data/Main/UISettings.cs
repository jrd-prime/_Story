using _StoryGame.Data.UI;
using UnityEngine;

namespace _StoryGame.Data.Main
{
    [CreateAssetMenu(
        fileName = nameof(UISettings),
        menuName = SOPathConst.MainSettings + nameof(UISettings),
        order = 100)]
    public sealed class UISettings : ASettingsBase
    {
        public UIViewerFloatingWindowsData FloatingWindowDataVo;
    }
}
