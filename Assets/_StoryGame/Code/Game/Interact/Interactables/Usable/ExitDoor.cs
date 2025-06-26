using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Data.Const;
using _StoryGame.Game.Interact.Abstract;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _StoryGame.Game.Interact.Interactables.Usable
{
    public sealed class ExitDoor : AUsable, IUsableExit
    {
        public string ExitQuestionLocalizationKey => LocalizationKey + LocalizationConst.ExitQuestionPostfix;

        protected override void OnStart()
        {
            var collider = GetComponent<Collider>() ?? throw new Exception($"Door {name} has no collider");

            CheckDoorLayer();
            SetUseAction(EUseAction.RoomExit);
        }

        private void CheckDoorLayer()
        {
            int requiredLayer = 31;
            if (gameObject.layer != requiredLayer)
            {
                string layerName = LayerMask.LayerToName(requiredLayer);
                string currentLayerName = LayerMask.LayerToName(gameObject.layer);

                throw new Exception(
                    $"Door {name} has wrong layer. " +
                    $"Should be layer {requiredLayer} ({layerName}), " +
                    $"but is {gameObject.layer} ({currentLayerName})"
                );
            }
        }

        public override async UniTask InteractAsync(ICharacter character)
        {
            var rotation = gameObject.transform.rotation;

            gameObject.transform.DORotate(
                new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y + 20, rotation.eulerAngles.z), 2,
                RotateMode.Fast);

            await System.Process(this);

            gameObject.transform.DORotate(
                new Vector3(rotation.eulerAngles.x, 0, rotation.eulerAngles.z), 2,
                RotateMode.Fast);
        }
    }
}
