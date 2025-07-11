using System;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Currency;
using _StoryGame.Core.HSM;
using _StoryGame.Core.HSM.Impls;
using _StoryGame.Core.HSM.Messages;
using _StoryGame.Core.Input.Messages;
using _StoryGame.Core.Managers;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.Providers.Settings;
using _StoryGame.Core.Room;
using _StoryGame.Core.WalletNew.Interfaces;
using _StoryGame.Data.Const;
using _StoryGame.Data.Loot;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.Managers.Interfaces;
using _StoryGame.Game.Managers.Room.Messages;
using _StoryGame.Game.Room.Abstract;
using _StoryGame.Game.UI.Impls.Viewer.Messages;
using _StoryGame.Infrastructure.AppStarter;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _StoryGame.Game.Managers.Game
{
    public sealed class GameManager : MonoBehaviour, IGameManager, IInitializable
    {
        public IWallet TempWallet { get; private set; }
        public IWallet PlayerWallet { get; private set; }
        public ReactiveProperty<GameState> CurrentGameState { get; }
        public ReactiveProperty<float> GameTime { get; }

        private ISettingsProvider _settingsManager;
        private HSM _hsm;
        private IGameService _gameService;
        private IJLog _log;
        private IPlayer _player;
        private ICameraManager _cameraManager;
        private IWalletService _walletService;

        private EnableInputMsg _enableInputCachedMsg;
        private DisableInputMsg _disableInputCachedMsg;
        private AppStartHandler _appStarter;

        private readonly CompositeDisposable _disposables = new();
        private ISubscriber<IGameManagerMsg> _gameManagerMsgSub;
        private IJPublisher _publisher;
        private IL10nProvider _il10NProvider;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _publisher = resolver.Resolve<IJPublisher>();
            _hsm = resolver.Resolve<HSM>();
            _settingsManager = resolver.Resolve<ISettingsProvider>();
            _gameService = resolver.Resolve<IGameService>();
            _log = resolver.Resolve<IJLog>();
            _player = resolver.Resolve<IPlayer>();
            _cameraManager = resolver.Resolve<ICameraManager>();
            _appStarter = resolver.Resolve<AppStartHandler>();
            _walletService = resolver.Resolve<IWalletService>();
            _gameManagerMsgSub = resolver.Resolve<ISubscriber<IGameManagerMsg>>();
            _il10NProvider = resolver.Resolve<IL10nProvider>(); //TODO not here
        }

        public void Initialize()
        {
            _cameraManager.SetTarget(_player);
            TempWallet = _walletService.GetOrCreate(ProjectConstant.TempWalletId);

            _player.SetEnergy(10);

            PlayerWallet = _player.Wallet;

            _enableInputCachedMsg = new EnableInputMsg();
            _disableInputCachedMsg = new DisableInputMsg();

            _appStarter.IsAppStarted
                .Subscribe(OnAppStarted)
                .AddTo(_disposables);

            _gameManagerMsgSub.Subscribe(
                OnSpendEnergyMsg,
                msg => msg is SpendEnergyMsg
            );

            _gameManagerMsgSub.Subscribe(
                OnTakeRoomLootMsg,
                msg => msg is TakeRoomLootMsg
            );

            _gameManagerMsgSub.Subscribe(
                OnTransitionToRoomRequestMsg,
                msg => msg is GoToRoomRequestMsg
            );
        }


        public bool IsPlayerHasItem(string itemId) => _player.Wallet.Has(itemId, 1);

        public bool IsPlayerHasConditionalItems(ACurrencyData[] conditionalItems)
        {
            foreach (var currencyData in conditionalItems)
            {
                if (!TempWallet.Has(currencyData.Id, 1))
                    return false;
            }

            return true;
        }

        private void OnTransitionToRoomRequestMsg(IGameManagerMsg obj)
        {
            Debug.Log($"OnMessage: {obj.GetType().Name}");
            var msg = obj as GoToRoomRequestMsg ?? throw new ArgumentNullException(nameof(obj));
            _publisher.ForRoomsDispatcher(new ChangeRoomRequestMsg(msg.ToExit, msg.FromRoom, msg.ToRoom));
        }

        private void OnSpendEnergyMsg(IGameManagerMsg message)
        {
            Debug.Log($"OnMessage: {message.GetType().Name}");
            var msg = message as SpendEnergyMsg ?? throw new ArgumentNullException(nameof(message));
            _player.SpendEnergy(msg.Amount);
        }

        private void OnTakeRoomLootMsg(IGameManagerMsg message)
        {
            Debug.Log($"OnMessage: {message.GetType().Name}");
            var msg = message as TakeRoomLootMsg ?? throw new ArgumentNullException(nameof(message));

            foreach (var lootDataNew in msg.ObjLoot.InspectablesLoot)
                ProcessLoot(lootDataNew);
        }

        //TODO ужас
        private void ProcessLoot(PreparedLootVo preparedLootVo)
        {
            switch (preparedLootVo.Currency.Type)
            {
                case ECurrencyType.Energy:
                    _player.AddEnergy(preparedLootVo.Currency.Amount);
                    break;
                case ECurrencyType.CoreItem:
                    // TempWallet.Add(preparedLootVo.Currency.Id, preparedLootVo.Currency.Amount);
                    _player.Wallet.Add(preparedLootVo.Currency.Id, preparedLootVo.Currency.Amount);
                    break;
                case ECurrencyType.Note:
                    _player.AddNote(preparedLootVo);
                    var noteTitle = _il10NProvider.Localize(preparedLootVo.Currency.LocalizationKey,
                        ETable.SimpleNote,
                        ETextTransform.Upper);
                    var noteText = _il10NProvider.Localize(
                        (preparedLootVo.Currency as ANoteData)?.GetTextLocalizationKey(), ETable.SimpleNote);
                    _publisher.ForUIViewer(new ShowNewNoteMsg(preparedLootVo, noteTitle, noteText));
                    break;
                case ECurrencyType.CoreNote:
                    _player.AddNote(preparedLootVo);
                    var title = _il10NProvider.Localize(preparedLootVo.Currency.LocalizationKey, ETable.CoreNote,
                        ETextTransform.Upper);
                    var text = _il10NProvider.Localize(
                        (preparedLootVo.Currency as ANoteData)?.GetTextLocalizationKey(), ETable.CoreNote);
                    _publisher.ForUIViewer(new ShowNewNoteMsg(preparedLootVo, title, text));
                    break;
                case ECurrencyType.Tip:
                    _log.Warn("Show tip");
                    break;
                case ECurrencyType.Special:
                    _log.Warn("Process special loot");
                    _player.Wallet.Add(preparedLootVo.Currency.Id, 1);
                    break;
                default:
                    _log.Error($"Unknown currency type: {preparedLootVo.Currency.Type}");
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnAppStarted(Unit _)
        {
            _log.Info("App started!");
            _gameService.StartHSM();
            _publisher.ForRoomsDispatcher(new ChangeRoomRequestMsg(EExit.B1B2Ladder, ERoom.NotSet,
                ERoom.SurfaceAccessModuleB1));
        }


        public void GameOver()
        {
            _log.Info("<color=red>GAME OVER</color>");
            _gameService.GameOver();
        }

        public void EndGame()
        {
            _log.Info("<color=red>GAME STOPPED</color>");
            _gameService.StopTheGame();
        }

        public void StartGame()
        {
            _log.Info("<color=green>GAME STARTED</color>");
            _gameService.StartNewGame();
            _publisher.ForInput(_enableInputCachedMsg);
        }

        public void PauseGame()
        {
            _log.Info("GAME PAUSED");
            _gameService.Pause();
            _publisher.ForInput(_disableInputCachedMsg);
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            _log.Info("GAME UNPAUSED");
            _gameService.UnPause();
            _publisher.ForInput(_enableInputCachedMsg);
            Time.timeScale = 1;
        }

        public void ContinueGame()
        {
            _log.Info("GAME CONTINUED");
            _gameService.ContinueGame();
        }

        public void SaveGame()
        {
        }

        public void LoadGame()
        {
        }
    }
}
