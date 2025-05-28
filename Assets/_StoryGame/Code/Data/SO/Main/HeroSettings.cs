using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.SO.Main
{
    [CreateAssetMenu(
        fileName = nameof(HeroSettings),
        menuName = SOPathConst.MainSettings + nameof(HeroSettings),
        order = 100)]
    public class HeroSettings : ASettingsBase
    {
        [field: SerializeField] public float Speed { get; private set; } = 5f;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 5f;
    }
}
