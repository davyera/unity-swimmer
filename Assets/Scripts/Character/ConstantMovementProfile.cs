using UnityEngine;

public class ConstantMovementProfile : MonoBehaviour, IMovementProfile {
    [SerializeField] private float _rotationDelayTime;
    [SerializeField] private float _snappyRotationDelayTime;

    [SerializeField] private float _groundTopSpeed;
    [SerializeField] private float _timeToGroundTopSpeed;

    [SerializeField] private float _swimTopSpeed;
    [SerializeField] private float _timeToSwimTopSpeed;
    [SerializeField] private float _swimTwistRotationMax;

    [SerializeField] private float _floatTerminalSpeed;

    public float RotationDelayTime { get => _rotationDelayTime; }
    public float SnappyRotationDelayTime { get => _snappyRotationDelayTime; }
    public float GroundTopSpeed { get => _groundTopSpeed; }
    public float TimeToGroundTopSpeed { get => _timeToGroundTopSpeed; }
    public float SwimTopSpeed { get => _swimTopSpeed; }
    public float TimeToSwimTopSpeed { get => _timeToSwimTopSpeed; }
    public float SwimTwistRotationMax { get => _swimTwistRotationMax; }
    public float FloatTerminalSpeed { get => _floatTerminalSpeed; }

}