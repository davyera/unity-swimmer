using UnityEngine;

public class HidingState : CharacterControlState {

    private readonly Vector3 _targetVelocity = Vector3.zero;

    private Vector3 _movementSmoothVelocity;

    public override float RotationDelayTime { get => MovementProfile.RotationDelayTime; }

    public HidingState(ICharacter character) : base(character) { }

    public override void Tick(ICharacterInput input) {
        Vector3 currentVelocity = Transform.Velocity;

        // Smoothly slow down to zero movement
        if (currentVelocity.magnitude > 0) {
            Vector3 finalVelocity = Vector3.SmoothDamp(
                currentVelocity,
                _targetVelocity,
                ref _movementSmoothVelocity,
                MovementProfile.TimeToGroundTopSpeed);

            Vector3 finalMovement = finalVelocity * Time.deltaTime;

            Transform.Move(finalMovement);
        }
    }

    public override void OnEnter() {
        base.OnEnter();

        _movementSmoothVelocity = Vector3.zero;
    }
}
