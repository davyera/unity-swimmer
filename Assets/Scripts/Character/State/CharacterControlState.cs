using UnityEngine;
public abstract class CharacterControlState : IState {

    private Timer _timer;

    private Vector3 _currentForwardLook;
    private Vector3 _currentUpLook;

    protected ICharacter Character { get; private set; }
    protected ICharacterInput Input { get; private set; }
    protected IMovementProfile MovementProfile { get; private set; }
    protected Transform Perspective { get; private set; }

    protected ITransformControl Transform { get; private set; }

    protected CharacterControlState(ICharacter character) {
        Character = character;
        MovementProfile = character.MovementProfile;
        Input = character.Input;
        Transform = character.Transform;
        Perspective = character.PerspectiveTransform;
        _timer = new Timer();
    }

    public virtual void OnEnter() { 
        _timer.Reset();
        _currentForwardLook = Transform.Get.forward;
        _currentUpLook = Transform.Get.up;
    }

    public virtual void OnExit() { }

    public void Tick() => Tick(Character.Input);

    public abstract void Tick(ICharacterInput input);

    public abstract float RotationDelayTime { get; }

    protected float GetElapsedTime() => _timer.Elapsed;

    protected Vector3 GetFlatMovementInput() {
        float lateral = Input.Horizontal;
        float forward = Input.Vertical;
        return ClampInput(new Vector3(lateral, 0, forward));
    }

    protected Vector3 GetMovementInput() {
        float lateral = Input.Horizontal;
        float forward = Input.Vertical;
        float vertical = 0;
        if (Input.JumpHeld)
            vertical = 1;
        else if (Input.DiveHeld)
            vertical = -1;

        return ClampInput(new Vector3(lateral, vertical, forward));
    }

    private Vector3 ClampInput(Vector3 input) => Vector3.ClampMagnitude(input, 1);

    protected Quaternion GetPerspectiveLookRotation() {
        (Vector3 targetForward, Vector3 targetUp) = GetPerspectiveForwardAndUp();
        return GetLookRotation(targetForward, targetUp, RotationDelayTime);
    }

    protected Quaternion GetPerspectiveLookRotationAlongGround(Vector3 groundNormal) =>
        GetPerspectiveLookRotationAlongGround(groundNormal, RotationDelayTime);

    protected Quaternion GetPerspectiveLookRotationAlongGround(Vector3 groundNormal, float smoothTime) {
        (Vector3 perspectiveForward, _) = GetPerspectiveForwardAndUp(groundNormal);
        Vector3 targetForwardLook = Vector3.ProjectOnPlane(perspectiveForward, groundNormal);
        Vector3 targetUpLook = groundNormal;

        return GetLookRotation(targetForwardLook, targetUpLook, smoothTime);
    }

    protected Quaternion GetLookRotationAlongGround(Vector3 groundNormal) {
        Vector3 targetForwardLook;
        Vector3 targetUpLook;
        if (Transform.Velocity.magnitude < 0.1f) {
            targetForwardLook = _currentForwardLook;
            targetUpLook = _currentUpLook;
        }
        else {
            Vector3 movementDirection = Transform.Velocity.normalized;
            targetForwardLook = Vector3.ProjectOnPlane(movementDirection, groundNormal);
            targetUpLook = groundNormal;
        }

        return GetLookRotation(targetForwardLook, targetUpLook, RotationDelayTime);
    }

    protected Quaternion GetLookRotation(
        Vector3 targetForward,
        Vector3 targetUp,
        float smoothTime) {

        // Rotate to a closer targetForward if the angle of change is over 90 degrees
        float forwardAngleWithCurrent = Vector3.Angle(targetForward, _currentForwardLook);
        if (forwardAngleWithCurrent >= 90) {
            // Figure out how we should rotate targetForward, left or right
            float forwardAngleWithRight = Vector3.Angle(targetForward, Transform.Get.right);
            float targetForwardRotationAngle = forwardAngleWithRight < 90 ? 90 : -90;

            Quaternion forwardLookFixRotation = Quaternion.AngleAxis(targetForwardRotationAngle, _currentUpLook);

            targetForward = forwardLookFixRotation * _currentForwardLook;
        }

        Vector3 forwardVelocity = Transform.ForwardLookVelocity;
        _currentForwardLook = Vector3.SmoothDamp(_currentForwardLook, targetForward, ref forwardVelocity, smoothTime);
        Transform.ForwardLookVelocity = forwardVelocity;

        Vector3 upVelocity = Transform.UpLookVelocity;
        _currentUpLook = Vector3.SmoothDamp(_currentUpLook, targetUp, ref upVelocity, smoothTime);
        Transform.UpLookVelocity = upVelocity;

        return Quaternion.LookRotation(_currentForwardLook, _currentUpLook);
    }

    protected (Vector3, Vector3) GetPerspectiveForwardAndUp() => (Perspective.forward, Perspective.up);

    protected (Vector3, Vector3) GetPerspectiveForwardAndUp(Vector3 groundNormal) {
        float forwardAngleWithGround = Vector3.Angle(Perspective.forward, groundNormal);
        float groundSlopeAngle = Vector3.Angle(groundNormal, Vector3.up);

        // If we are "climbing" we treat our perspective differently
        if (groundSlopeAngle >= 45)
            // When perspective is "overhead" from the transform
            if (forwardAngleWithGround >= 90)
                return (Vector3.up, groundNormal);
            // Otherwise, perspective is "underneath" the transform
            else
                return (-Vector3.up, groundNormal);

        return GetPerspectiveForwardAndUp();
    }

}