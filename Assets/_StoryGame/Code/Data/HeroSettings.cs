using UnityEngine;

namespace _StoryGame.Data
{
    [CreateAssetMenu(
        fileName = nameof(HeroSettings),
        menuName = SOPathConst.MainSettings + nameof(HeroSettings),
        order = 100)]
    public class HeroSettings : SettingsBase
    {
        public override string ConfigName { get; } = nameof(HeroSettings);
        [field: SerializeField] public float Speed { get; private set; } = 5f;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 5f;
    }
}
