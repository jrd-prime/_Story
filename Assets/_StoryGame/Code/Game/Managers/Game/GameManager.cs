using System;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.HSM.Impls;
using _StoryGame.Core.Interfaces;
using _StoryGame.Core.Interfaces.Managers;
using _StoryGame.Core.WalletNew.Interfaces;
using _StoryGame.Data.Const;
using _StoryGame.Game.Loot.Impls;
using _StoryGame.Game.Managers.Inerfaces;
using _StoryGame.Infrastructure.AppStarter;
using _StoryGame.Infrastructure.Input.Interfaces;
using _StoryGame.Infrastructure.Input.Messages;
using _StoryGame.Infrastructure.Logging;
using _StoryGame.Infrastructure.Settings;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using IGameManager = _StoryGame.Core.Interfaces.Managers.IGameManager;

namespace _StoryGame.Game.Managers.Impls
{
    public sealed class GameManager : MonoBehaviour, IGameManager, IInitializable
    {
        public IWallet TempWallet { get; private set; }

        private ISettingsProvider _settingsManager;
        private HSM _hsm;
        private IGameService _gameService;
        private IPublisher<IInputMsg> _inputPublisher;
        private IJLog _log;
        private IPlayer _player;
        private ICameraManager _cameraManager;
        private IWalletService _walletService;

        private EnableInputMsg _enableInputCachedMsg;
        private DisableInputMsg _disableInputCachedMsg;
        private AppStartHandler _appStarter;

        private readonly CompositeDisposable _disposables = new();
        private ISubscriber<IGameManagerMsg> _gameManagerMsgSub;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _inputPublisher = resolver.Resolve<IPublisher<IInputMsg>>();
            _hsm = resolver.Resolve<HSM>();
            _settingsManager = resolver.Resolve<ISettingsProvider>();
            _gameService = resolver.Resolve<IGameService>();
            _log = resolver.Resolve<IJLog>();
            _player = resolver.Resolve<IPlayer>();
            _cameraManager = resolver.Resolve<ICameraManager>();
            _appStarter = resolver.Resolve<AppStartHandler>();
            _walletService = resolver.Resolve<IWalletService>();
            _gameManagerMsgSub = resolver.Resolve<ISubscriber<IGameManagerMsg>>();
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

            _gameManagerMsgSub.Subscribe(OnSpendEnergyMsg, msg => msg is SpendEnergyMsg);
            _gameManagerMsgSub.Subscribe(OnTakeRoomLootMsg, msg => msg is TakeRoomLootMsg);
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

            foreach (var VARIABLE in msg.Loot.InspectablesLoot)
            {
                Debug.Log(VARIABLE.Currency.LocalizationKey);
                TempWallet.Add(VARIABLE.Currency.Id, VARIABLE.Currency.Amount);
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
            _inputPublisher.Publish(_enableInputCachedMsg);
        }

        public void Pause()
        {
            _log.Info("GAME PAUSED");
            _gameService.Pause();
            _inputPublisher.Publish(_disableInputCachedMsg);
            Time.timeScale = 0;
        }

        public void UnPause()
        {
            _log.Info("GAME UNPAUSED");
            _gameService.UnPause();
            _inputPublisher.Publish(_enableInputCachedMsg);
            Time.timeScale = 1;
        }

        public void ContinueGame()
        {
            _log.Info("GAME CONTINUED");
            _gameService.ContinueGame();
        }
    }

    public record SpendEnergyMsg(int Amount) : IGameManagerMsg
    {
        public int Amount { get; } = Amount;
    }

    public record TakeRoomLootMsg(InspectableData Loot) : IGameManagerMsg
    {
        public InspectableData Loot { get; } = Loot;
    }

    public interface IGameManagerMsg : IJMessage
    {
    }
}
