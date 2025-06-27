using System;
using System.Collections.Generic;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.HSM.Impls.States.Gameplay;
using _StoryGame.Core.HSM.Impls.States.Menu;
using _StoryGame.Core.HSM.Interfaces;
using _StoryGame.Core.HSM.Messages;
using MessagePipe;
using R3;

namespace _StoryGame.Core.HSM.Impls
{
    /// <summary>
    /// Hierarchical State Machine
    /// </summary>
    public sealed class HSM : IDisposable
    {
        public Observable<EGameStateType> CurrentStateType => _currentStateType;

        private IState _currentState;
        private IState _previousState;
        private readonly ReactiveProperty<EGameStateType> _currentStateType = new();
        private readonly Dictionary<EGameStateType, IState> _states = new();

        private readonly CompositeDisposable _disposables = new();
        private readonly IJLog _log;

        public HSM(ISubscriber<IHSMMsg> hsmSubscriber, IJLog log)
        {
            _log = log;
            InitializeMainStates();
            hsmSubscriber.Subscribe(HandleMessage).AddTo(_disposables);
        }

        /// <summary>
        /// Создаем экземпляры глобальных состояний и регистрируем
        /// </summary>
        private void InitializeMainStates()
        {
            RegisterState<MenuState>(new MenuState(this), EGameStateType.Menu);
            RegisterState<GameplayState>(new GameplayState(this), EGameStateType.Gameplay);
        }

        /// <summary>
        /// Запуск дефолтного состояния
        /// </summary>
        public void Start()
        {
            var rootState = _states[EGameStateType.Gameplay];
            // rootState = _states[EGameStateType.Menu];
            _previousState = null;
            _currentState = rootState;
            _currentStateType.Value = _currentState.StateType;
            _currentState.Enter(_previousState);
        }

        /// <summary>
        /// Обновление состояния
        /// </summary>
        public void Update()
        {
            _log.Warn($"<color=green>Update!");
            _currentState.Update();

            var nextState = _currentState.HandleTransition();

            if (nextState != null && nextState != _currentState)
                TransitionTo(nextState.StateType);
        }

        /// <summary>
        /// Смена состояния
        /// </summary>
        private void TransitionTo(EGameStateType stateType)
        {
            if (!_states.TryGetValue(stateType, out var newState))
                throw new Exception($"state {stateType} not found");

            _log.Info($"{_currentStateType.Value.GetType().Name} > {newState.GetType().Name}");
            _previousState = _currentState;
            _currentState.Exit(_previousState);
            _currentState = newState;
            _currentStateType.Value = _currentState.StateType;
            _currentState.Enter(_previousState);
        }

        /// <summary>
        /// Регистрация глобального состояния
        /// </summary>
        private void RegisterState<T>(IState state, EGameStateType stateType) where T : IState =>
            _states[stateType] = state;

        private void HandleMessage(IHSMMsg msg)
        {
            switch (msg)
            {
                case ChangeGameStateMessage stateMessage:
                    TransitionTo(stateMessage.StateType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(msg));
            }
        }

        public void Dispose()
        {
            _currentStateType?.Dispose();
            _disposables?.Dispose();
        }
    }
}
