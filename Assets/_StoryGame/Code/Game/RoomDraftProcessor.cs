using System.Threading.Tasks;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Core.UI;
using _StoryGame.Game.Managers.Game.Messages;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Unity.VisualScripting;
using UnityEngine;

namespace System.Runtime.CompilerServices.Game
{
    public sealed class RoomDraftProcessor
    {
        private readonly IRoomsRegistry _roomsRegistry;
        private readonly IJPublisher _publisher;
        private readonly IJLog _log;
        private readonly DialogResultHandler _dialogResultHandler;

        public RoomDraftProcessor(IJPublisher publisher, IJLog log, IRoomsRegistry roomsRegistry,
            ISubscriber<IGameManagerMsg> gameManagerMsgSub)
        {
            _log = log;
            _publisher = publisher;
            gameManagerMsgSub.Subscribe(OnChooseNextRoomRequestMsg);
            _roomsRegistry = roomsRegistry;

            _dialogResultHandler = new DialogResultHandler(_log);
            _dialogResultHandler.AddCallback(EDialogResult.Apply, OnRoomChosen);
            _dialogResultHandler.AddCallback(EDialogResult.Close, OnCancel);
        }

        private void OnRoomChosen()
        {
            _log.Warn(" RoomChosen");
        }

        private void OnCancel()
        {
            _log.Warn("Room Draft Cancel");
        }

        private void OnChooseNextRoomRequestMsg(IGameManagerMsg msg)
        {
            var message = msg as ChooseNextRoomRequestMsg ?? throw new ArgumentNullException(nameof(msg));
            Debug.LogWarning("RoomDraftProcessor.OnChooseNextRoomRequestMsg = " + message);

            ShowVariants().Forget();
        }

        private async UniTask ShowVariants()
        {
            await DisplayDraftUI();
        }

        private async UniTask DisplayDraftUI()
        {
            var source = new UniTaskCompletionSource<DraftResult>();
            DraftVariantsData variants = GetVariants();

            _publisher.ForPlayerOverHeadUI(new DisplayRoomDraftWindowMsg("RoomDraft", variants, source));

            var result = await source.Task;
            _dialogResultHandler.HandleResult(result.DialogResult);
        }

        private DraftVariantsData GetVariants()
        {
            return new DraftVariantsData(new DraftVariantData[] { });
        }

        private void AddRoomToPool()
        {
        }

        private void RemoveRoomFromPool()
        {
        }
    }

    public record DraftResult(EDialogResult DialogResult, int VariantIndex);

    public record DisplayRoomDraftWindowMsg(
        string Roomdraft,
        DraftVariantsData DraftVariantsData,
        UniTaskCompletionSource<DraftResult> CompletionSource)
        : IPlayerOverHeadUIMsg;

    public record DraftVariantsData(DraftVariantData[] Variants);

    public record DraftVariantData(int VariantIndex, string VariantName, string VariantDescription);
}
