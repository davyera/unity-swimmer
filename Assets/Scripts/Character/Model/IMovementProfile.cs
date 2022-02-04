public interface IMovementProfile {
    // General movement
    float RotationDelayTime { get; }
    float SnappyRotationDelayTime { get; }

    // Ground movement
    float GroundTopSpeed { get; }
    float TimeToGroundTopSpeed { get; }

    // Swim movement
    float SwimTopSpeed { get; }
    float TimeToSwimTopSpeed { get; }

    float SwimTwistRotationMax { get; }

    // Float movement
    float FloatTerminalSpeed { get; }
}