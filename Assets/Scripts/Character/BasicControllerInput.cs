using UnityEngine;

public class BasicControllerInput : MonoBehaviour, ICharacterInput {

    [SerializeField] private float _movementTolerance = 0.1f;

    public float Horizontal { get; private set; }

    public float Vertical { get; private set; }

    public bool JumpRequested { get; private set; } 

    public bool JumpHeld { get; private set; }

    public bool DiveRequested { get; private set; }

    public bool DiveHeld { get; private set; }

    private void Update() {
        Horizontal = GetAxisThreshold("Horizontal");
        Vertical = GetAxisThreshold("Vertical");

        JumpRequested = Input.GetButtonDown("Jump");
        if (JumpRequested) JumpHeld = true;
        if (Input.GetButtonUp("Jump")) JumpHeld = false;

        DiveRequested = Input.GetButtonDown("Fire1");
        if (DiveRequested) DiveHeld = true;
        if (Input.GetButtonUp("Fire1")) DiveHeld = false;
    }

    private float GetAxisThreshold(string axisName) {
        float axisValue = Input.GetAxis(axisName);
        return Mathf.Abs(axisValue) > _movementTolerance ? axisValue : 0;
    }
}