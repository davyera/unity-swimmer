using System;

public interface IStateMachine {
    void InitState(IState state);

    void Tick();

    void AddTransition(IState from, IState to, Func<bool> predicate);

    void AddTransition(IState to, Func<bool> predicate);

    void DebugLogTransitions();
}
