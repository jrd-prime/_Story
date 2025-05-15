using UnityEngine;

namespace _StoryGame.Data.Interactables
{
    [CreateAssetMenu(menuName = "Databases/" + nameof(CollectableObjData), fileName = nameof(CollectableObjData))]
    public class CollectableObjData : InteractableSettings
    {
        public override string ConfigName => nameof(CollectableObjData);
    }
}
