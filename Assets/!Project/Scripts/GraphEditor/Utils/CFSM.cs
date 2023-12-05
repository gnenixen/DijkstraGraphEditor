using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

public interface IState {
    public void OnRegistered();
    public void OnEnter();
    public void OnExit();
}

public class CFSM<T, K> where T : IState {
    public event Action<T> OnTransitionEvent = delegate { };

    private readonly Dictionary<K, T> mStates = new();

    public T CurrentState { get; private set; }

    public T RegisterState(K key, T state) {
        Assert.IsFalse(mStates.ContainsKey(key), "FSM already contains given key.");

        mStates.Add(key, state);
        state.OnRegistered();

        return state;
    }

    public void Transition(K to) {
        Assert.IsTrue(mStates.ContainsKey(to));

        CurrentState?.OnExit();
        CurrentState = mStates[to];
        CurrentState?.OnEnter();

        OnTransitionEvent.Invoke(CurrentState);
    }
}