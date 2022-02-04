using UnityEngine;

public class CharacterControllerCollider : MonoBehaviour, ICollisionDetector {

    [SerializeField] private float _senseDistanceMultiple = 10;
    [SerializeField] private float _groundReactionMultiple = 4;

    private CharacterController _characterController;

    private RaycastRef _ground;

    void Start() {
        _ground = RaycastRef.Miss;
        _characterController = GetComponent<CharacterController>();
    }

    void Update() {
        UpdateGround();
    }

    private void UpdateGround() {
        float rayDistance = SenseRayDistance();
        Ray ray = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, GlobalState.GroundLayerMask))
            _ground = new RaycastRef(hit);
        else
            _ground = RaycastRef.Miss;
    }

    public bool IsHeadBlocked => (_characterController.collisionFlags & CollisionFlags.Above) != 0;

    public bool IsGrounded => _ground.WasHit && _ground.Distance <= BoundRadius + BoundCushion * 2;

    public RaycastRef GetGround() => _ground;

    public bool IsGroundWithinRange() =>
        _ground.WasHit && _ground.Distance < BoundRadius * _groundReactionMultiple;

    public RaycastRef SenseCollision() { 
        Vector3 transformVelocity = _characterController.velocity;
        if (transformVelocity.magnitude < 0.1)
            return RaycastRef.Miss;

        float rayDistance = SenseRayDistance();
        Ray ray = new Ray(transform.position, transformVelocity.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, GlobalState.GroundLayerMask))
            return new RaycastRef(hit);
        else
            return RaycastRef.Miss;
    }

    private float SenseRayDistance() => BoundRadius * _senseDistanceMultiple;

    public float BoundRadius { get => _characterController.radius; }

    public float BoundCushion { get => _characterController.skinWidth; }
}