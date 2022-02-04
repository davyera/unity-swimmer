using UnityEngine;

public class Timer {
    private float _startTime;

    public float Elapsed { get => Time.time - _startTime;  }

    public Timer() => Reset();

    public void Reset() => _startTime = Time.time;

}