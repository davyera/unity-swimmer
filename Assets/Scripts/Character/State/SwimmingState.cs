using UnityEngine;

public class SwimmingState : CharacterControlState {

    private Vector3 _movementSmoothVelocity;

    private float _rollAngle;
    private float _rollAngleVelocity;

    private float _pitchAngle;
    private float _pitchAngleVelocity;

    private float _yawAngle;
    private float _yawAngleVelocity;

    public override float RotationDelayTime { get => MovementProfile.SnappyRotationDelayTime; }

    public SwimmingState(ICharacter character) : base(character) { }

    public override void Tick(ICharacterInput input) {
        Vector3 localMovementInput = GetMovementInput();
        float movementInputMagnitude = localMovementInput.magnitude;
        Vector3 localMovementDirection = localMovementInput.normalized;

        Transform perspective = Character.PerspectiveTransform;
        Transform transform = Transform.Get;

        Vector3 movementDirection = perspective.rotation * localMovementDirection;

        Vector3 currentVelocity = Transform.Velocity;
        Vector3 targetVelocity =
            movementDirection * movementInputMagnitude * MovementProfile.SwimTopSpeed;
        Vector3 finalVelocity = Vector3.SmoothDamp(
            currentVelocity,
            targetVelocity,
            ref _movementSmoothVelocity,
            MovementProfile.TimeToSwimTopSpeed);

        Vector3 finalMovement = finalVelocity * Time.deltaTime;

        Transform.Move(finalMovement);

        // Derive twist rotations 
        // direction of twist rotations depends on if we are moving forward or backward
        int twistSign = localMovementInput.z >= 0 ? 1 : -1;
        Vector3 twistVector = twistSign * localMovementInput;

        Quaternion rollRotation = GetTwistRotation(twistVector.x, transform.forward, ref _rollAngle, ref _rollAngleVelocity);
        Quaternion pitchRotation = GetTwistRotation(twistVector.y, transform.right, ref _pitchAngle, ref _pitchAngleVelocity);
        Quaternion yawRotation = GetTwistRotation(-twistVector.x * 0.7f, transform.up, ref _yawAngle, ref _yawAngleVelocity);

        Quaternion baseRotation;
        RaycastRef collision = Character.CollisionDetector.SenseCollision();
        if (collision.WasHit)
            baseRotation = GetPerspectiveLookRotationAlongGround(collision.Normal, MovementProfile.RotationDelayTime);
        else
            baseRotation = GetPerspectiveLookRotation();

        Quaternion finalRotation = yawRotation * pitchRotation * rollRotation * baseRotation;

        Transform.Rotate(finalRotation);
    }

    private Quaternion GetTwistRotation(
        float contributingMovement, 
        Vector3 rotationAxis, 
        ref float angle, 
        ref float angleVelocity) {

        float targetAngle = -contributingMovement * MovementProfile.SwimTwistRotationMax;
        angle = Mathf.SmoothDamp(angle, targetAngle, ref angleVelocity, MovementProfile.RotationDelayTime);
        return Quaternion.AngleAxis(angle, rotationAxis);
    }

    public override void OnEnter() {
        base.OnEnter();

        _rollAngle = 0;
        _rollAngleVelocity = 0;

        _pitchAngle = 0;
        _pitchAngleVelocity = 0;

        _yawAngle = 0;
        _yawAngleVelocity = 0;
    }
}