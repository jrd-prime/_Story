using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Game.Interactables.Abstract;
using _StoryGame.Game.Interactables.Impls.Systems;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interactables.Impls.Use
{
    public sealed class RoomDoor : AUsable
    {
        [SerializeField] private GameObject door;
        [SerializeField] private string exitQuestionLocalizationKey;
        private UseSystem _useSystem;

        public string ExitQuestionLocalizationKey => exitQuestionLocalizationKey;

        protected override void ResolveDependencies(IObjectResolver resolver) =>
            _useSystem = resolver.Resolve<UseSystem>();

        public override async UniTask InteractAsync(ICharacter character)
        {
            var rotation = door.transform.rotation;

            door.transform.DORotate(
                new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y + 20, rotation.eulerAngles.z), 2,
                RotateMode.Fast);

            await _useSystem.Process(this);

            door.transform.DORotate(
                new Vector3(rotation.eulerAngles.x, 0, rotation.eulerAngles.z), 2,
                RotateMode.Fast);
        }
    }
}
