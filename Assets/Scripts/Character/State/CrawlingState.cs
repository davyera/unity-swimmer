using System;
using UnityEngine;

public class CrawlingState : CharacterControlState {

    private ICollisionDetector _bounds;
    private Transform _transform;

    private Vector3 _movementSmoothVelocity;

    private IRaycastSweeper _groundAnticipation;

    private Vector3 _localMovementDirection;
    private float _movementInputMagnitude;

    private Vector3 _perspectiveForward;
    private Vector3 _perspectiveUp;

    public override float RotationDelayTime { get => MovementProfile.SnappyRotationDelayTime; }

    public CrawlingState(ICharacter character) : base(character) {
        _bounds = character.CollisionDetector;
        _transform = Transform.Get;

        InitGroundAnticipationSweeper();
    }

    private void InitGroundAnticipationSweeper() {
        Func<float> radiusFn = () =>_bounds.BoundRadius;

        Func<Vector3> positionFn = () => _transform.position;
        Func<Vector3> heightFn = () => _transform.up * radiusFn();
        Func<Vector3> movementFn = 
            () => Quaternion.LookRotation(_perspectiveForward, _perspectiveUp) * _localMovementDirection * radiusFn();

        Func<Vector3> overhead =                () => positionFn() + heightFn();
        Func<Vector3> underneath =              () => positionFn() - heightFn() * 2;
        Func<Vector3> anticipatedPosition =     () => positionFn() + movementFn();
        Func<Vector3> anticipatedGround =       () => anticipatedPosition() - heightFn();
        Func<Vector3> anticipatedUnderneath =   () => anticipatedPosition() - heightFn() * 2;

        Func<(Vector3, Vector3)>[] pointFuncs = new Func<(Vector3, Vector3)>[] {
            // First ray from overhead to future position, for sensing walls
            () => (overhead(), anticipatedPosition()),
            // Second ray from future position downwards, basic anticipation raycast
            () => (anticipatedPosition(), anticipatedGround()),
            // Third ray from future position towards underneath, for cliffs
            () => (anticipatedPosition(), underneath()),
            // Fourth ray from future "underneath", towards underneath, for peaks
            () => (anticipatedUnderneath(), underneath())
        };

        Func<float> rayDistanceFunc = () => radiusFn() * 2;
        _groundAnticipation = new RaycastSweeper(rayDistanceFunc, GlobalState.GroundLayerMask, pointFuncs);
        _groundAnticipation.DebugRays(false);
    }

    public override void Tick(ICharacterInput input) {
        ReadControllerInput();

        RaycastRef directGround = _bounds.GetGround();

        (_perspectiveForward, _perspectiveUp) = GetPerspectiveForwardAndUp(directGround.Normal);

        RaycastRef anticipatedGround = GetAnticipatedGround(directGround);

        // Check if anticipated ground is "above" transform's perspective.
        //  If so, we want to use the anticipate ground details for our movement vector to handle walls.
        //  When anticipated ground is "below" perspective, we move along direct ground
        //      and let 'groundErrorFix' bring us back down.
        RaycastRef ground;
        if (MathUtils.IsFurther(anticipatedGround.Point, directGround.Point, _transform.up))
            ground = anticipatedGround;
        else
            ground = directGround;

        // Re-orient movement to ground normal
        Quaternion perspectiveRotation = Quaternion.LookRotation(_perspectiveForward, _perspectiveUp);
        Vector3 movementToPerspective = perspectiveRotation * _localMovementDirection;
        Vector3 perspectiveMapped = Vector3.ProjectOnPlane(movementToPerspective, ground.Normal);
        Vector3 movementDirection = perspectiveMapped.normalized;

        // Derive small velocity to ensure we stay stuck on the ground
        Vector3 groundErrorFix = CalculateGroundErrorFixVelocity(ground.Distance, ground.Point);

        // Calculate slope slowdown effects
        float slopeEffector = CalculateSlopeEffect(ground.Normal);

        float topSpeed = MovementProfile.GroundTopSpeed;
        Vector3 targetVelocity = movementDirection * _movementInputMagnitude * slopeEffector * topSpeed;

        Vector3 currentVelocity = Transform.Velocity;
        float time = MovementProfile.TimeToGroundTopSpeed;
        Vector3 smoothVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref _movementSmoothVelocity, time);

        Vector3 finalVelocity = smoothVelocity + groundErrorFix;
        Vector3 finalMovement = finalVelocity * Time.deltaTime;
        Transform.Move(finalMovement);

        Quaternion finalRotation = GetLookRotationAlongGround(anticipatedGround.Normal);
        Transform.Rotate(finalRotation);
    }

    private void ReadControllerInput() {
        Vector3 localMovementInput = GetFlatMovementInput();
        _movementInputMagnitude = localMovementInput.magnitude;
        _localMovementDirection = localMovementInput.normalized;
    }

    private RaycastRef GetAnticipatedGround(RaycastRef directGroundHit) {
        if (_groundAnticipation.Sweep(out RaycastHit anticipatedGroundHit))
            return new RaycastRef(directGroundHit, anticipatedGroundHit);
        else
            return new RaycastRef(directGroundHit); 
    }

    private float CalculateSlopeEffect(Vector3 groundNormal) {
        float slopeAngle = Vector3.Angle(groundNormal, Vector3.up);

        // No slope effect if under 45 degrees
        if (slopeAngle <= 45)
            return 1;
        
        // Cut speed up to half if traversing 90 degree incline
        float coefficient = MathUtils.Normalize(slopeAngle, 45, 90);
        float multiplier = 1 - coefficient / 2;
        return multiplier;
    }

    private Vector3 CalculateGroundErrorFixVelocity(float groundDistance, Vector3 groundPoint) {
        float gap = groundDistance - _bounds.BoundRadius;
        if (gap <= _bounds.BoundCushion) 
            return Vector3.zero;

        Vector3 groundDirection = (groundPoint - _transform.position).normalized;
        float fixSpeed = 0.005f;
        return groundDirection * fixSpeed;
    }
}