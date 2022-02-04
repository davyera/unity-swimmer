
public interface ICharacterInput {

    float Horizontal { get; }
    float Vertical { get; }
    bool JumpRequested { get; }
    bool JumpHeld { get; }
    bool DiveRequested { get; }
    bool DiveHeld { get; }
}
