using System;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Currency;
using _StoryGame.Core.HSM.Impls;
using _StoryGame.Core.Input.Messages;
using _StoryGame.Core.Managers;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.Providers.Localization;
using _StoryGame.Core.Providers.Settings;
using _StoryGame.Core.WalletNew.Interfaces;
using _StoryGame.Data.Const;
using _StoryGame.Data.Loot;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Game.Interactables.Impls.Systems;
using _StoryGame.Game.Managers.Game.Messages;
using _StoryGame.Game.Managers.Interfaces;
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
        public ReactiveProperty<GameState> CurrentGameState { get; }
        public ReactiveProperty<float> GameTime { get; }

        public void StartGame()
        {
            throw new NotImplementedException();
        }

        public void PauseGame()
        {
            throw new NotImplementedException();
        }

        public void ResumeGame()
        {
            throw new NotImplementedException();
        }

        public void EndGame()
        {
            throw new NotImplementedException();
        }

        public void SaveGame()
        {
            throw new NotImplementedException();
        }

        public void LoadGame()
        {
            throw new NotImplementedException();
        }

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
        private ILocalizationProvider _localizationProvider;

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
            _localizationProvider = resolver.Resolve<ILocalizationProvider>(); //TODO not here
        }

        public void Initialize()
        {
            _cameraManager.SetTarget(_player);
            TempWallet = _walletService.GetOrCreate(ProjectConstant.TempWalletId);
            Debug.LogWarning("TempWallet: " + TempWallet);

            _player.SetEnergy(10);


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
                OnRoomChooseRequestMsg,
                msg => msg is RoomChooseRequestMsg
            );
        }

        private void OnRoomChooseRequestMsg(IGameManagerMsg obj)
        {
            Debug.Log($"OnMessage: {obj.GetType().Name}");
            var msg = obj as RoomChooseRequestMsg ?? throw new ArgumentNullException(nameof(obj));
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

            foreach (var lootDataNew in msg.Loot.InspectablesLoot)
                ProcessLoot(lootDataNew);
        }

        //TODO ужас
        private void ProcessLoot(InspectableLootData lootData)
        {
            switch (lootData.Currency.Type)
            {
                case ECurrencyType.Energy:
                    _player.AddEnergy(lootData.Currency.Amount);
                    break;
                case ECurrencyType.CoreItem:
                    TempWallet.Add(lootData.Currency.Id, lootData.Currency.Amount);
                    break;
                case ECurrencyType.Note:
                    _player.AddNote(lootData);
                    var noteTitle = _localizationProvider.Localize(lootData.Currency.LocalizationKey,
                        ETable.SimpleNote,
                        ETextTransform.Upper);
                    var noteText = _localizationProvider.Localize(
                        (lootData.Currency as ANoteData)?.GetTextLocalizationKey(), ETable.SimpleNote);
                    _publisher.ForUIViewer(new ShowNewNoteMsg(lootData, noteTitle, noteText));
                    break;
                case ECurrencyType.CoreNote:
                    _player.AddNote(lootData);
                    var title = _localizationProvider.Localize(lootData.Currency.LocalizationKey, ETable.CoreNote,
                        ETextTransform.Upper);
                    var text = _localizationProvider.Localize(
                        (lootData.Currency as ANoteData)?.GetTextLocalizationKey(), ETable.CoreNote);
                    _publisher.ForUIViewer(new ShowNewNoteMsg(lootData, title, text));
                    break;
                case ECurrencyType.Tip:
                    _log.Warn("Show tip");
                    break;
                case ECurrencyType.Special:
                    _log.Warn("Process special loot");
                    break;
                default:
                    _log.Error($"Unknown currency type: {lootData.Currency.Type}");
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnAppStarted(Unit _)
        {
            _log.Info("App started!");
            _gameService.StartHSM();
        }


        public void GameOver()
        {
            _log.Info("<color=red>GAME OVER</color>");
            _gameService.GameOver();
        }

        public void StopTheGame()
        {
            _log.Info("<color=red>GAME STOPPED</color>");
            _gameService.StopTheGame();
        }

        public void StartNewGame()
        {
            _log.Info("<color=green>GAME STARTED</color>");
            _gameService.StartNewGame();
            _publisher.ForInput(_enableInputCachedMsg);
        }

        public void Pause()
        {
            _log.Info("GAME PAUSED");
            _gameService.Pause();
            _publisher.ForInput(_disableInputCachedMsg);
            Time.timeScale = 0;
        }

        public void UnPause()
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
    }
}
