using System;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Game.Managers.Room.Messages;
using MessagePipe;
using VContainer.Unity;

namespace _StoryGame.Game.Managers.Room
{
    public sealed class RoomsDispatcher : IRoomsDispatcher, IInitializable
    {
        private IRoom _currentRoom = null;

        private readonly IRoomsRegistry _roomsRegistry;
        private readonly IJLog _log;
        private readonly IPlayer _player;

        public RoomsDispatcher(IRoomsRegistry roomsRegistry, IJLog log, IPlayer player,
            ISubscriber<IRoomsDispatcherMsg> roomsDispatcherMsgSub)
        {
            _roomsRegistry = roomsRegistry;
            _log = log;
            _player = player;

            roomsDispatcherMsgSub.Subscribe(OnRoomsDispatcherMsg);
        }

        public void Initialize()
        {
            _log.Debug("Rooms in registry: " + _roomsRegistry.GetRoomsCount());
        }

        private void OnRoomsDispatcherMsg(IRoomsDispatcherMsg message)
        {
            switch (message)
            {
                case ChangeRoomRequestMsg msg:
                    ChangeRoomTo(msg);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message), message, null);
            }
        }

        private void ChangeRoomTo(ChangeRoomRequestMsg msg)
        {
            _log.Debug("Changing room to: " + msg.ToRoom);
            _currentRoom?.Hide();
            _currentRoom = _roomsRegistry.GetRoomByType(msg.ToRoom);
            _currentRoom.Show();

            var exitSpawnPosition = _currentRoom.GetExitPointFor(msg.Exit).GetEntryPoint();

            _player.SetPosition(exitSpawnPosition);
        }
    }

    public interface IRoomsDispatcher
    {
    }
}
