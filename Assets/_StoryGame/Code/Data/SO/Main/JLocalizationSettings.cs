using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.SO.Main
{
    [CreateAssetMenu(
        fileName = nameof(JLocalizationSettings),
        menuName = SOPathConst.MainSettings + nameof(JLocalizationSettings),
        order = 0)]
    public sealed class JLocalizationSettings : ASettingsBase
    {
        [field: SerializeField] public Language DefaultLanguage { get; private set; } = Language.English;
    }

    public enum Language
    {
        English = 0,
        Russian = 1
    }
}
