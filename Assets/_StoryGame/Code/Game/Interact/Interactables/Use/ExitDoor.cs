using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Room;
using _StoryGame.Data.Const;
using _StoryGame.Game.Interact.Abstract;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace _StoryGame.Game.Interact.Interactables.Use
{
    public sealed class ExitDoor : AUsable, IUsableExit
    {
        [SerializeField] private ERoom roomType = ERoom.NotSet;
        [SerializeField] private ERoom transitionToRoom = ERoom.NotSet;
        [SerializeField] private EDoorRotation doorRotation;

        [SerializeField] private EDoorAction doorAction;

        [FormerlySerializedAs("doorData")] [SerializeField]
        private OpenableObjData openableObjData;

        public string ExitQuestionLocalizationKey => LocalizationKey + LocalizationConst.ExitQuestionPostfix;

        public ERoom RoomType => roomType;
        public ERoom TransitionToRoom => transitionToRoom;
        public EDoorAction DoorAction => doorAction;


        protected override void Enable()
        {
        }

        protected override void OnStart()
        {
            var collider = GetComponent<Collider>() ?? throw new Exception($"Door {name} has no collider");

            CheckDoorLayer();

            if (roomType == ERoom.NotSet || roomType == transitionToRoom)
                throw new Exception($"Door {name} has invalid room type. Not set or equal to transition to room.");

            if (doorAction == EDoorAction.NotSet)
                throw new Exception("DoorAction not set. " + name);

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

        protected override void OnAwake()
        {
        }

        public override async UniTask InteractAsync(ICharacter character)
        {
            var rotation = gameObject.transform.rotation;

            // gameObject.transform.DORotate(
            //     new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y + 20, rotation.eulerAngles.z), 2,
            //     RotateMode.Fast);

            await System.Process(this);

            // gameObject.transform.DORotate(
            //     new Vector3(rotation.eulerAngles.x, 0, rotation.eulerAngles.z), 2,
            //     RotateMode.Fast);
        }
    }


    public enum EDoorAction
    {
        NotSet = -1,
        EnterQ = 0,
        ExitQ = 1,
        AscendQ = 2,
        DescendQ = 3
    }

    public enum EDoorRotation
    {
        NotSet = -1,
        RightIn = 0,
        RightOut = 1,
        LeftIn = 2,
        LeftOut = 3,
        Up = 4,
        Down = 5
    }
}
