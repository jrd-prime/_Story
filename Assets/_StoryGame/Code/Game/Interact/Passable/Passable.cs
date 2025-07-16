using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.WalletNew.Messages;
using _StoryGame.Data.Interact;
using _StoryGame.Game.Interact.Abstract;
using _StoryGame.Game.Interact.Passable.Systems;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.Serialization;

namespace _StoryGame.Game.Interact.Passable
{
    /// <summary>
    /// Проходимые
    /// </summary>
    public sealed class Passable : AConditional<PassSystem>, IPassable
    {
        [FormerlySerializedAs("openableObjData")] [FormerlySerializedAs("doorData")] [SerializeField]
        private PassableData passableData;

        public EPassableState PassableState { get; private set; } = EPassableState.NotSet;

        public PassableData PassableData => passableData;

        protected override void OnAwake()
        {
        }

        protected override void OnStart()
        {
            if (!passableData)
                throw new Exception("PassableData is null. " + gameObject.name);

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
            PassableState = conditionsResult.Success ? EPassableState.Unlocked : EPassableState.Locked;
            // _log.Warn("PassableState: " + PassableState + " / name = " + gameObject.name);
        }

        public override async UniTask InteractAsync(ICharacter character)
        {
            if (System == null)
                throw new Exception("ConditionalSystem is null. " + gameObject.name);

            await System.Process(this);
        }

        public void SetState(EPassableState state) => PassableState = state;
    }
}
