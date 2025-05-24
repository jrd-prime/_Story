using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.HSM.Impls;
using _StoryGame.Core.Interfaces.Managers;
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

namespace _StoryGame.Game.Managers.Impls
{
    public class GameManager : MonoBehaviour, IGameManager, IInitializable
    {
        private ISettingsProvider _settingsManager;
        private HSM _hsm;
        private IGameService _gameService;
        private IPublisher<IInputMessage> _inputPublisher;
        private IJLog _log;
        private IPlayer _player;
        private ICameraManager _cameraManager;

        private EnableInputMessage _enableInputCachedMessage;
        private DisableInputMessage _disableInputCachedMessage;
        private AppStartHandler _appStarter;

        private readonly CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _inputPublisher = resolver.Resolve<IPublisher<IInputMessage>>();
            _hsm = resolver.Resolve<HSM>();
            _settingsManager = resolver.Resolve<ISettingsProvider>();
            _gameService = resolver.Resolve<IGameService>();
            _log = resolver.Resolve<IJLog>();
            _player = resolver.Resolve<IPlayer>();
            _cameraManager = resolver.Resolve<ICameraManager>();
            _appStarter = resolver.Resolve<AppStartHandler>();
        }

        public void Initialize()
        {
            _cameraManager.SetTarget(_player);

            _enableInputCachedMessage = new EnableInputMessage();
            _disableInputCachedMessage = new DisableInputMessage();

            _appStarter.IsAppStarted
                .Subscribe(OnAppStarted)
                .AddTo(_disposables);
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
            _inputPublisher.Publish(_enableInputCachedMessage);
        }

        public void Pause()
        {
            _log.Info("GAME PAUSED");
            _gameService.Pause();
            _inputPublisher.Publish(_disableInputCachedMessage);
            Time.timeScale = 0;
        }

        public void UnPause()
        {
            _log.Info("GAME UNPAUSED");
            _gameService.UnPause();
            _inputPublisher.Publish(_enableInputCachedMessage);
            Time.timeScale = 1;
        }

        public void ContinueGame()
        {
            _log.Info("GAME CONTINUED");
            _gameService.ContinueGame();
        }
    }
}
