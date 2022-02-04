using UnityEngine;

public class FloatingState : CharacterControlState {

    private float _gravity = 9.81f / 10;

    private ICollisionDetector _bounds;

    private readonly Vector3 _targetVelocity = Vector3.zero;

    private Vector3 _movementSmoothVelocity;

    private Vector3 _movementVelocity;
    private Vector3 _gravityVelocity;

    public override float RotationDelayTime { get => MovementProfile.RotationDelayTime; }

    public FloatingState(ICharacter character) : base(character) {
        _bounds = Character.CollisionDetector;
    }

    public override void Tick(ICharacterInput input) {
        _movementVelocity = Vector3.SmoothDamp(
            _movementVelocity,
            _targetVelocity,
            ref _movementSmoothVelocity,
            MovementProfile.TimeToSwimTopSpeed);

        _gravityVelocity += Vector3.down * _gravity * Time.deltaTime;
        _gravityVelocity = Vector3.ClampMagnitude(_gravityVelocity, MovementProfile.FloatTerminalSpeed);

        Vector3 finalVelocity = _movementVelocity + _gravityVelocity;
        Vector3 finalMovement = finalVelocity * Time.deltaTime;

        Transform.Move(finalMovement);

        Quaternion finalRotation;

        if (_bounds.IsGroundWithinRange())
            finalRotation = GetPerspectiveLookRotationAlongGround(_bounds.GetGround().Normal);
        else
            finalRotation = GetPerspectiveLookRotation();

        Transform.Rotate(finalRotation);
    }

    public override void OnEnter() {
        base.OnEnter();
        _movementVelocity = Transform.Velocity;
        _gravityVelocity = Vector3.zero;
    }
}