using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTransitionStateMachine : IStateMachine {
    private IState _currentState;

    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> _currentTransitions = new List<Transition>();
    private List<Transition> _fromAnyTransitions = new List<Transition>();
    private static List<Transition> NoTransitions = new List<Transition>(0);

    public void InitState(IState initState) {
        if (_currentState == null)
            SetState(initState);
    }
    
    private void SetState(IState newState) {
        if (_currentState == newState) 
            return;

        _currentState?.OnExit();
        _currentState = newState;

        CacheTransitions(newState);
        newState.OnEnter();

        Debug.Log("Transitioning to " + newState.GetType());
    }

    public void AddTransition(IState from, IState to, Func<bool> onCondition) {
        var fromType = from.GetType();
        if (!_transitions.TryGetValue(fromType, out var transitions)) {
            transitions = new List<Transition>();
            _transitions[fromType] = transitions;
        }
        AddTransition(transitions, to, onCondition);
    }

    public void AddTransition(IState to, Func<bool> onCondition) {
        AddTransition(_fromAnyTransitions, to, onCondition);
    }

    private void AddTransition(List<Transition> transitions, IState to, Func<bool> onCondition) {
        transitions.Add(new Transition(to, onCondition));
    }

    private void CacheTransitions(IState newState) {
        _transitions.TryGetValue(newState.GetType(), out _currentTransitions);
        if (_currentTransitions == null)
            _currentTransitions = NoTransitions;
    }

    public void Tick() {
        var transition = GetTransition();
        if (transition != null)
            SetState(transition.To);

        _currentState?.Tick();
    }

    private class Transition {
        public Func<bool> Condition { get; }
        public IState To { get; }

        public Transition(IState to, Func<bool> onCondition) {
            To = to;
            Condition = onCondition;
        }
    }

    private Transition GetTransition() {
        foreach (var transition in _fromAnyTransitions)
            if (transition.Condition())
                return transition;

        foreach (var transition in _currentTransitions)
            if (transition.Condition())
                return transition;

        return null;
    }

    public void DebugLogTransitions() {
        string logString = "from ANY transitions:";
        foreach (var transition in _fromAnyTransitions) {
            logString += "\n\tto " + transition.To.GetType();
        }
        logString += "\ndirect transitions:";
        foreach(var transitionFromTo in _transitions) {
            logString += "\n\tfrom " + transitionFromTo.Key;
            foreach(var transition in transitionFromTo.Value) {
                logString += "\n\t\tto " + transition.To.GetType();
            }
        }
        Debug.Log(logString);
    }
}
