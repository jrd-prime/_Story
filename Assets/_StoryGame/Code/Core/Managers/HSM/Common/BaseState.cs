using System;
using System.Collections.Generic;
using _StoryGame.Core.Managers.HSM.Interfaces;

namespace _StoryGame.Core.Managers.HSM.Common
{
    public abstract class BaseState : IState
    {
        public IState PreviousState;
        
        protected internal readonly Impls.HSM StateMachine;
        protected IState CurrentSubState;
        protected readonly Dictionary<Type, IState> SubStates = new();

        protected BaseState(Impls.HSM stateMachine)
        {
            StateMachine = stateMachine;
        }

        public virtual void Enter(IState previousState)
        {
            CurrentSubState?.Enter(previousState);
        }

        public virtual void Exit(IState previousState)
        {
            CurrentSubState?.Exit(previousState);
        }

        public virtual void Update()
        {
            if (CurrentSubState == null) return;

            CurrentSubState.Update();
            var nextSubState = CurrentSubState.HandleTransition();
            if (nextSubState != null && nextSubState != CurrentSubState)
                TransitionToSubState(nextSubState);
        }

        public virtual IState HandleTransition() => null;
        protected void AddSubState<T>(IState state) where T : IState => SubStates[typeof(T)] = state;
        protected void SetInitialSubState<T>() where T : IState => CurrentSubState = SubStates[typeof(T)];

        protected void TransitionToSubState<T>() where T : IState
        {
            if (SubStates.TryGetValue(typeof(T), out IState newState))
                TransitionToSubState(newState);
            else throw new Exception($"state {typeof(T).Name} not found");
        }

        private void TransitionToSubState(IState newState)
        {
            if (newState == CurrentSubState) return;

            PreviousState = CurrentSubState;
            CurrentSubState?.Exit(PreviousState);
            CurrentSubState = newState;
            CurrentSubState?.Enter(PreviousState);
        }
    }
}
