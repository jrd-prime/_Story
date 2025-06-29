using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.UI;
using _StoryGame.Game.Interact.Interactables.Usable;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Game.Interact.Interactables.Unlock
{
    public sealed class UnlockableDoor : AUnlockable
    {
        [SerializeField] private DoorData doorData;
        [SerializeField] private EDoorState doorState = EDoorState.NotSet;

        public EDoorState DoorState => doorState;

        protected override void OnStart()
        {
            var conditionsResult = CheckConditionsToUnlock();

            doorState = conditionsResult.Success ? EDoorState.Unlocked : EDoorState.Locked;
        }

        protected override void OnSomeEnable()
        {
        }

        public override async UniTask InteractAsync(ICharacter character)
        {
            if (System == null)
                throw new Exception("ConditionalSystem is null. " + gameObject.name);

            await System.Process(this);
        }
    }

    public enum EDoorState
    {
        NotSet = -1,
        Locked,
        Unlocked
    }
}
