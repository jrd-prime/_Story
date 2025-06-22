using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Game.Interact.Abstract;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _StoryGame.Game.Interact.ObjTypes.Usable
{
    public sealed class RoomDoor : AUsable
    {
        [SerializeField] private GameObject door;
        [SerializeField] private string exitQuestionLocalizationKey;

        public string ExitQuestionLocalizationKey => exitQuestionLocalizationKey;

        public override async UniTask InteractAsync(ICharacter character)
        {
            var rotation = door.transform.rotation;

            door.transform.DORotate(
                new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y + 20, rotation.eulerAngles.z), 2,
                RotateMode.Fast);

            await System.Process(this);

            door.transform.DORotate(
                new Vector3(rotation.eulerAngles.x, 0, rotation.eulerAngles.z), 2,
                RotateMode.Fast);
        }
    }
}
