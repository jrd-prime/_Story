using System;
using System.Collections.Generic;
using _StoryGame.Core.Managers.HSM.Impls.States.Gameplay;
using _StoryGame.Core.Managers.HSM.Impls.States.Menu;
using _StoryGame.Core.Managers.HSM.Interfaces;
using _StoryGame.Core.Managers.HSM.Messages;
using _StoryGame.Infrastructure.Logging;
using MessagePipe;
using R3;

namespace _StoryGame.Core.Managers.HSM.Impls
{
    /// <summary>
    /// Hierarchical State Machine
    /// </summary>
    public sealed class HSM : IDisposable
    {
        public Observable<IState> CurrentState => _currentState;

        private IState _previousState;
        private readonly ReactiveProperty<IState> _currentState = new(null);
        private readonly Dictionary<Type, IState> _states = new();

        private readonly CompositeDisposable _disposables = new();
        private readonly IJLog _log;

        public HSM(ISubscriber<IHSMMessage> hsmSubscriber, IJLog log)
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
            RegisterState<MenuState>(new MenuState(this));
            RegisterState<GameplayState>(new GameplayState(this));
        }

        /// <summary>
        /// Запуск дефолтного состояния
        /// </summary>
        public void Start()
        {
            var rootState = _states[typeof(MenuState)];
            _previousState = null;
            _currentState.Value = rootState;
            _currentState.CurrentValue.Enter(_previousState);
        }

        /// <summary>
        /// Обновление состояния
        /// </summary>
        public void Update()
        {
            _log.Warn($"<color=green>[{nameof(HSM)}]</color> Update!");
            _currentState.Value.Update();

            var nextState = _currentState.Value.HandleTransition();

            if (nextState != null && nextState != _currentState.Value)
                TransitionTo(nextState.GetType());
        }

        /// <summary>
        /// Смена состояния
        /// </summary>
        private void TransitionTo<T>(T stateType) where T : Type
        {
            if (!_states.TryGetValue(stateType, out var newState))
                throw new Exception($"state {stateType.Name} not found");

            _log.Info(
                $"<color=green>[{nameof(HSM)}]</color> {_currentState.Value.GetType().Name} > {newState.GetType().Name}");
            _previousState = _currentState.Value;
            _currentState.Value.Exit(_previousState);
            _currentState.Value = newState;
            _currentState.Value.Enter(_previousState);
        }

        /// <summary>
        /// Регистрация глобального состояния
        /// </summary>
        private void RegisterState<T>(IState state) where T : IState => _states[typeof(T)] = state;

        private void HandleMessage(IHSMMessage msg)
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
            _currentState?.Dispose();
            _disposables?.Dispose();
        }
    }
}
