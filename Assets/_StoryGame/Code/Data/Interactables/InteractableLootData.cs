using UnityEngine;

namespace _StoryGame.Data.Interactables
{
    [CreateAssetMenu(
        fileName = nameof(InteractableLootData),
        menuName = SOPathConst.Interactables + nameof(InteractableLootData)
    )]
    public sealed class InteractableLootData : SettingsBase
    {
        public override string ConfigName { get; }
    }
}
