using UnityEngine;

namespace _StoryGame.Data
{
    [CreateAssetMenu(
        fileName = nameof(JLocalizationSettings),
        menuName = SOPathConst.MainSettings + nameof(JLocalizationSettings),
        order = 0)]
    public sealed class JLocalizationSettings : SettingsBase
    {
        public override string ConfigName { get; }
        [field: SerializeField] public Language DefaultLanguage { get; private set; } = Language.English;
    }

    public enum Language
    {
        English = 0,
        Russian = 1
    }
}
