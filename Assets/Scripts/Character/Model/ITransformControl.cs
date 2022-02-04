using UnityEngine;

public interface ITransformControl {

    Transform Get { get; }

    void Move(Vector3 movement);

    Vector3 Velocity { get; }

    Quaternion Rotation { get; }

    void Rotate(Quaternion rotation);

    Vector3 ForwardLookVelocity { get; set; }

    Vector3 UpLookVelocity { get; set; }
}