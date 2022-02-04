using UnityEngine;

public interface ICharacter {

    ICharacterInput Input { get; }

    IMovementProfile MovementProfile { get; }

    ITransformControl Transform { get; }

    ICollisionDetector CollisionDetector { get; }

    Transform PerspectiveTransform { get; }

}
