using _StoryGame.Data.Const;
using _StoryGame.Data.SO.Abstract;
using UnityEngine;

namespace _StoryGame.Data.SO.Interactables
{
    [CreateAssetMenu(
        fileName = nameof(InteractableLootData),
        menuName = SOPathConst.Interactables + nameof(InteractableLootData)
    )]
    public sealed class InteractableLootData : ASettingsBase
    {
    }
}
