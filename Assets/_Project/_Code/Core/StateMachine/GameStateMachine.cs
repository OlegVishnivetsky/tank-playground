using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace TankPlayground.Core.StateMachine
{
    public class GameStateMachine : IGameStateMachine, IDisposable
    {
        private readonly Dictionary<Type, IState> _states = new();

        private readonly Subject<IState> _currentStateSubject = new();
        public Observable<IState> CurrentStateObservable => _currentStateSubject;
        public IState CurrentState { get; private set; }

        private readonly IStateFactory _stateFactory;

        public GameStateMachine(IStateFactory stateFactory) => _stateFactory = stateFactory;

        public void Dispose()
        {
            _currentStateSubject?.Dispose();

            if (CurrentState is IExitState stateToExit)
                stateToExit.Exit();
        }

        public void RegisterState<TState>(TState state) where TState : class, IState
        {
            Type stateType = typeof(TState);

            if (_states.ContainsKey(stateType))
            {
                _states.Remove(stateType);
                Debug.LogWarning($"[StateMachine]: Removed existing state {stateType.Name}");
            }

            _states.Add(stateType, state);
            Debug.Log($"[StateMachine]: {stateType.Name} Registered");
        }

        public void SwitchTo<TState>() where TState : class, IState
        {
            if (!_states.TryGetValue(typeof(TState), out IState state))
            {
                state = _stateFactory.Create<TState>();
                _states.Add(typeof(TState), state);
            }
            
            if (state is IStateWithCondition stateWithCondition && !stateWithCondition.CanBeEntered())
                return;
            
            if (CurrentState is IExitState stateToExit)
                stateToExit.Exit();
            
            CurrentState = state;
            _currentStateSubject.OnNext(state);

            if (state is IEnterState stateToEnter)
                stateToEnter.Enter();

            Debug.Log($"[StateMachine]: {state.GetType().Name} Entered");
        }

        public void SwitchTo<TState, TParameter>(TParameter parameter)
            where TState : class, IEnterStateWithParameter<TParameter>
        {
            if (!_states.TryGetValue(typeof(TState), out IState state))
            {
                state = _stateFactory.Create<TState>();
                _states.Add(typeof(TState), state);
            }
            
            if (state is IStateWithCondition stateWithCondition && !stateWithCondition.CanBeEntered())
                return;
            
            if (CurrentState is IExitState stateToExit)
                stateToExit.Exit();

            CurrentState = state;
            _currentStateSubject.OnNext(state);

            if (state is IEnterStateWithParameter<TParameter> stateToEnter)
                stateToEnter.Enter(parameter);

            Debug.Log($"[StateMachine]: {state.GetType().Name} Entered with parameter");
        }
        
        public void SwitchTo<TState, TParameter1, TParameter2>(TParameter1 parameter1, TParameter2 parameter2)
            where TState : class, IEnterStateWithParameter<TParameter1, TParameter2>
        {
            if (!_states.TryGetValue(typeof(TState), out IState state))
            {
                state = _stateFactory.Create<TState>();
                _states.Add(typeof(TState), state);
            }
            
            if (state is IStateWithCondition stateWithCondition && !stateWithCondition.CanBeEntered())
                return;
            
            if (CurrentState is IExitState stateToExit)
                stateToExit.Exit();

            CurrentState = state;
            _currentStateSubject.OnNext(state);

            if (state is IEnterStateWithParameter<TParameter1, TParameter2> stateToEnter)
                stateToEnter.Enter(parameter1, parameter2);

            Debug.Log($"[StateMachine]: {state.GetType().Name} Entered with parameter");
        }
    }
}