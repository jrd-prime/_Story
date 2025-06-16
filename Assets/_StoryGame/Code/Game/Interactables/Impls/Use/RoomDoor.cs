using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Game.Interactables.Abstract;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interactables.Impls.Use
{
    public sealed class RoomDoor : AUsable
    {
        [SerializeField] private GameObject door;

        public override UniTask InteractAsync(ICharacter character)
        {
            door.transform.rotation = Quaternion.Euler(door.transform.rotation.x, 15, door.transform.rotation.z);
            return UniTask.CompletedTask;
        }
    }
}
