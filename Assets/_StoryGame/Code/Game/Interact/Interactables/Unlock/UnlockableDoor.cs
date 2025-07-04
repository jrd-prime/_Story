using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.WalletNew.Messages;
using _StoryGame.Game.Interact.Interactables.Use;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using UnityEngine;

namespace _StoryGame.Game.Interact.Interactables.Unlock
{
    public sealed class UnlockableDoor : AUnlockable
    {
        [SerializeField] private DoorData doorData;

        public EDoorState DoorState { get; private set; } = EDoorState.NotSet;

        public DoorData DoorData => doorData;

        protected override void OnStart()
        {
            if (!doorData)
                throw new Exception("DoorData is null. " + gameObject.name);

            InitCurrentState();
        }

        protected override void Subscribe()
        {
            // _log.Warn("Subscribe AUnlockable in " + gameObject.name);
            ItemLootedMsgSub.Subscribe(OnItemLooted).AddTo(_disposables);
        }

        protected override void Unsubscribe() => _disposables.Dispose();


        private void OnItemLooted(ItemAmountChangedMsg msg)
        {
            // _log.Warn("OnItemLooted. New amount: " + msg.Amount + ". Item: " + msg.ItemId);
            InitCurrentState();
        }

        private void InitCurrentState()
        {
            var conditionsResult = CheckConditionsToUnlock();
            DoorState = conditionsResult.Success ? EDoorState.Unlocked : EDoorState.Locked;
            // _log.Warn("DoorState: " + DoorState + " / name = " + gameObject.name);
        }

        public override async UniTask InteractAsync(ICharacter character)
        {
            if (System == null)
                throw new Exception("ConditionalSystem is null. " + gameObject.name);

            await System.Process(this);
        }

        public void SetState(EDoorState state) => DoorState = state; 
    }
}
