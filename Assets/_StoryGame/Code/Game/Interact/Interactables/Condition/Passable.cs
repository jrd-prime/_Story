using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.WalletNew.Messages;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.InteractableNew.Conditional;
using _StoryGame.Game.Interact.Interactables.Unlock;
using _StoryGame.Game.Interact.Interactables.Use;
using _StoryGame.Game.Interact.Systems;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.Serialization;

namespace _StoryGame.Game.Interact.Interactables.Condition
{
    /// <summary>
    /// Проходимые
    /// </summary>
    public sealed class Passable : AConditional<PassSystem>, IPassable
    {
        [FormerlySerializedAs("doorData")] [SerializeField]
        private OpenableObjData openableObjData;

        public EDoorState DoorState { get; private set; } = EDoorState.NotSet;

        public OpenableObjData OpenableObjData => openableObjData;

        protected override void OnAwake()
        {
        }

        protected override void OnStart()
        {
            if (!openableObjData)
                throw new Exception("OpenableObjData is null. " + gameObject.name);

            InitCurrentState();
        }

        protected override void Enable()
        {
        }

        protected override void Subscribe()
        {
            // _log.Warn("Subscribe AConditional in " + gameObject.name);
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


    public interface IPassable : IInteractable
    {
    }
}
