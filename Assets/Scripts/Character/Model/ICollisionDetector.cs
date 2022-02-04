using UnityEngine;

public interface ICollisionDetector {

    bool IsHeadBlocked { get; }

    bool IsGrounded { get; }

    RaycastRef GetGround();

    bool IsGroundWithinRange();

    RaycastRef SenseCollision();

    float BoundRadius { get; }

    float BoundCushion { get; }

}