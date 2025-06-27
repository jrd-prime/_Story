using System;
using System.Collections.Generic;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Interact.Enums;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Core.Loot.Interfaces;
using _StoryGame.Core.Managers;
using _StoryGame.Core.Providers.Settings;
using _StoryGame.Core.Room.Interfaces;
using _StoryGame.Data.Interact;
using _StoryGame.Data.Loot;
using _StoryGame.Data.Room;
using _StoryGame.Data.SO.Room;
using _StoryGame.Game.Interact.Interactables;
using _StoryGame.Game.Interact.Interactables.Usable;
using _StoryGame.Game.Room.Messages;
using _StoryGame.Infrastructure.AppStarter;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace _StoryGame.Game.Room.Abstract
{
    public abstract class ARoom : MonoBehaviour, IRoom
    {
        [SerializeField] private string roomId;
        [SerializeField] private string roomName;
        [SerializeField] private RoomInteractablesVo interactables;
        [SerializeField] private List<ExitDoor> doors;
        [SerializeField] private Transform spawnPoint;

        [SerializeField] private RoomExitVo[] exit;

        public string Id => roomId;
        public string Name => roomName;
        public float Progress { get; }
        public RoomLootVo Loot => _roomData.Loot;
        public RoomInteractablesVo Interactables => interactables;

        private RoomData _roomData;
        private IPublisher<RoomLootGeneratedMsg> _roomLootGeneratedMsgPub;
        private readonly CompositeDisposable _disposables = new();

        private readonly List<IConditional> _conditionalObjects = new();
        private IJLog _log;
        private IGameManager _gameManager;

        [Inject]
        private void Construct(IJLog log, ISettingsProvider settingsProvider,
            IPublisher<RoomLootGeneratedMsg> roomLootGeneratedMsgPub, AppStartHandler appStartHandler,
            IGameManager gameManager)
        {
            _log = log;
            _roomData = settingsProvider.GetRoomSettings(Id);
            _roomLootGeneratedMsgPub = roomLootGeneratedMsgPub;

            _gameManager = gameManager;
            appStartHandler.IsAppStarted
                .Subscribe(OnAppStarted).AddTo(_disposables);

            _gameManager.TempWallet.OnCurrencyChanged
                .Subscribe(_ => UpdateStateForConditionalObjects()).AddTo(_disposables);
            _gameManager.PlayerWallet.OnCurrencyChanged
                .Subscribe(_ => UpdateStateForConditionalObjects()).AddTo(_disposables);

            LoadConfig();
        }

        private void Awake()
        {
            SayMyNameToObjects();

            var conditionals =
                FindObjectsByType<Conditional>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            _conditionalObjects.AddRange(conditionals);

            UpdateStateForConditionalObjects();
        }

        private void OnAppStarted(Unit _)
        {
            _roomLootGeneratedMsgPub.Publish(new RoomLootGeneratedMsg(roomId));
        }


        private void LoadConfig()
        {
            if (!_roomData)
                throw new NullReferenceException($"Room {Id} not found in settings.");

            if (_roomData.Id != Id)
                throw new Exception($"Room {Id} settings is not correct.");
        }

        public InspectableLootVo GetLootData() => _roomData.Loot.inspectableLoot;

        // TODO call on game start
        public bool UpdateStateForConditionalObjects()
        {
            _log.Debug("<color=green>Update State For Conditional Objects</color>".ToUpper());

            if (_conditionalObjects.Count == 0)
            {
                _log.Error("Room has no conditional objects");
                return false;
            }

            foreach (var conditionalObject in _conditionalObjects)
            {
                var hasItem = _gameManager.IsPlayerHasItem(conditionalObject.GetSpecialItemId());
                var hasConditionItems = _gameManager.IsPlayerHasConditionalItems(conditionalObject.ConditionalItems);

                if (hasItem)
                    conditionalObject.SetConditionalState(EConditionalState.Looted);
                else if (hasConditionItems)
                    conditionalObject.SetConditionalState(EConditionalState.Unlocked);
                else
                    conditionalObject.SetConditionalState(EConditionalState.Locked);

                _log.Debug(
                    $"Init state for {conditionalObject.Id} / Player already has item: {hasItem} / Has items for unlock: {hasConditionItems}");
            }

            return true;
        }

        public Vector3 GetSpawnPosition() => spawnPoint.position;

        private void SayMyNameToObjects()
        {
            interactables.core.SetRoom(this);

            foreach (var conditional in interactables.hidden)
                conditional.SetRoom(this);

            foreach (var inspectable in interactables.inspectables)
                inspectable.SetRoom(this);
        }
    }

    [Serializable]
    public struct RoomExitVo
    {
        public ERoom toRoom;
        [FormerlySerializedAs("exit")] public ExitDoor exitDoor;
    }

    public enum ERoom
    {
        /// <summary>
        /// Exit from the bunker
        /// </summary>
        SurfaceAccessModule0,

        /// <summary>
        ///  Main corridor -1 floor
        /// </summary>
        CorridorMain1,

        /// <summary>
        /// Living quarters corridor -1 floor
        /// </summary>
        CorridorLivingQuarters1,

        /// <summary>
        /// Server room -1 floor
        /// </summary>
        ServerFacilityModule1,

        /// <summary>
        /// Hygiene module -1 floor
        /// </summary>
        HygieneModule1,

        /// <summary>
        /// Med module -1 floor
        /// </summary>
        MedModule1,

        /// <summary>        
        /// Relaxation module -1 floor
        /// </summary>
        RelaxationModule1,

        /// <summary>
        /// Nutrition module  -1 floor
        /// </summary>
        NutritionModule1,

        /// <summary>
        /// Habitation module -1 floor
        /// </summary>
        HabitationModule1,

        /// <summary>        
        /// Main corridor -2 floor
        /// </summary>
        CorridorMain2,

        /// <summary>
        /// Waste reclamation module -2 floor
        /// </summary>
        WasteReclamationModule2,

        /// <summary>
        /// Electro mechanical module -2 floor
        /// </summary>  
        ElectroMechanicalModule2,

        /// <summary>
        /// Climate control module -2 floor
        /// </summary>
        ClimateControlModule2,

        /// <summary>
        /// Vault -2 floor
        /// </summary>
        Vault2,

        /// <summary>
        /// Warehouse -2 floor
        /// </summary>
        Warehouse2
    }
}
