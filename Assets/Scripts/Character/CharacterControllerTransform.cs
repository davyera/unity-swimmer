using UnityEngine;

public class CharacterControllerTransform : MonoBehaviour, ITransformControl {
    
    private CharacterController _characterController;

    [SerializeField] private bool _enableOverlapRecovery = true;

    void Start() {
        _characterController = GetComponent<CharacterController>();
    }

    void Update() {
        _characterController.enableOverlapRecovery = _enableOverlapRecovery;
    }

    public Transform Get { get => transform; }

    public Vector3 Velocity { get => _characterController.velocity; }

    public Quaternion Rotation { get => transform.rotation; }

    public Vector3 ForwardLookVelocity { get; set; }

    public Vector3 UpLookVelocity { get; set; }

    public void Move(Vector3 movement) => _characterController.Move(movement);

    public void Rotate(Quaternion rotation) => transform.rotation = rotation;
}