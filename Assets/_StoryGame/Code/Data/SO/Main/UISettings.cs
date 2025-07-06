using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Data.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace _StoryGame.Data.SO.Main
{
    [CreateAssetMenu(
        fileName = nameof(UISettings),
        menuName = SOPathConst.MainSettings + nameof(UISettings),
        order = 100)]
    public sealed class UISettings : ASettingsBase
    {
        public UIViewerFloatingWindowsData FloatingWindowDataVo;
        public VisualTreeAsset inventoryHUDCellTemplate;
    }
}
