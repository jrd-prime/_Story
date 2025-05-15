using UnityEngine;

namespace _StoryGame.Data
{
    [CreateAssetMenu(
        fileName = nameof(UIViewsSettings),
        menuName = SOPathConst.MainSettings + nameof(UIViewsSettings),
        order = 100)]
    public class UIViewsSettings : SettingsBase
    {
        // [field: SerializeField] public UIStateData[] States { get; private set; }
        public override string ConfigName { get; }
    }
}
